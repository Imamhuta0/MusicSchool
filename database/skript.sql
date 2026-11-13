

CREATE TABLE roles (
    role_id int IDENTITY(1,1),
    role_name nvarchar(50) NOT NULL,
    CONSTRAINT pk_roles PRIMARY KEY (role_id),
    CONSTRAINT uq_roles_name UNIQUE (role_name)
);

CREATE TABLE categories (
    category_id int IDENTITY(1,1),
    category_name nvarchar(100) NOT NULL,
    CONSTRAINT pk_categories PRIMARY KEY (category_id),
    CONSTRAINT uq_categories_name UNIQUE (category_name)
);

CREATE TABLE users (
    user_id int IDENTITY(1,1),
    role_id int NOT NULL,
    login nvarchar(50) NOT NULL,
    password_hash nvarchar(255) NOT NULL,
    last_name nvarchar(60) NOT NULL,
    first_name nvarchar(60) NOT NULL,
    middle_name nvarchar(60) NULL,
    CONSTRAINT pk_users PRIMARY KEY (user_id),
    CONSTRAINT uq_users_login UNIQUE (login),
    CONSTRAINT fk_users_role FOREIGN KEY (role_id) REFERENCES roles(role_id)
);

CREATE TABLE lessons (
    lesson_id int IDENTITY(1,1),
    category_id int NOT NULL,
    title nvarchar(150) NOT NULL,
    description nvarchar(max) NULL,
    image_path nvarchar(150) NULL,
    base_price decimal(10,2) NOT NULL,
    CONSTRAINT pk_lessons PRIMARY KEY (lesson_id),
    CONSTRAINT fk_lessons_category FOREIGN KEY (category_id) REFERENCES categories(category_id),
    CONSTRAINT chk_lessons_price CHECK (base_price > 0)
);

CREATE TABLE lesson_variants (
    variant_id int IDENTITY(1,1),
    lesson_id int NOT NULL,
    teacher_id int NOT NULL,
    variant_name nvarchar(50) NOT NULL,
    available_seats int NOT NULL CONSTRAINT df_variants_seats DEFAULT (0),
    CONSTRAINT pk_lesson_variants PRIMARY KEY (variant_id),
    CONSTRAINT fk_variants_lesson FOREIGN KEY (lesson_id) REFERENCES lessons(lesson_id) ON DELETE CASCADE,
    CONSTRAINT fk_variants_teacher FOREIGN KEY (teacher_id) REFERENCES users(user_id),
    CONSTRAINT chk_variants_seats CHECK (available_seats >= 0)
);

CREATE TABLE orders (
    order_id int IDENTITY(1,1),
    order_date date NOT NULL CONSTRAINT df_orders_date DEFAULT (CAST(GETDATE() AS date)),
    client_id int NOT NULL,
    CONSTRAINT pk_orders PRIMARY KEY (order_id),
    CONSTRAINT fk_orders_client FOREIGN KEY (client_id) REFERENCES users(user_id)
);

CREATE TABLE order_items (
    order_item_id int IDENTITY(1,1),
    order_id int NOT NULL,
    variant_id int NOT NULL,
    quantity int NOT NULL,
    unit_price decimal(10,2) NOT NULL,
    lesson_date date NOT NULL CONSTRAINT df_order_items_lesson_date DEFAULT (CAST(GETDATE() AS date)),
    CONSTRAINT pk_order_items PRIMARY KEY (order_item_id),
    CONSTRAINT uq_order_item UNIQUE (order_id, variant_id),
    CONSTRAINT chk_items_quantity CHECK (quantity > 0),
    CONSTRAINT chk_items_price CHECK (unit_price >= 0),
    CONSTRAINT fk_items_order FOREIGN KEY (order_id) REFERENCES orders(order_id) ON DELETE CASCADE,
    CONSTRAINT fk_items_variant FOREIGN KEY (variant_id) REFERENCES lesson_variants(variant_id)
);
GO

CREATE INDEX ix_users_login ON users(login);
CREATE INDEX ix_variants_lesson ON lesson_variants(lesson_id);
CREATE INDEX ix_orders_client ON orders(client_id);
CREATE INDEX ix_items_variant ON order_items(variant_id);
GO

USE MusicSchoolDB;
GO

INSERT INTO roles (role_name) VALUES 
(N'Администратор'),
(N'Преподаватель'),
(N'Ученик');
GO

INSERT INTO categories (category_name) VALUES 
(N'Фортепиано'),
(N'Скрипка'),
(N'Гитара'),
(N'Вокал');
GO

INSERT INTO users (role_id, login, password_hash, last_name, first_name, middle_name) VALUES
(1, N'admin',        N'2121', N'Кириллова', N'Диана',  N'Евгеньевна'),
(2, N'fortepiano',   N'1111', N'Иванова',   N'Анна',   N'Петровна'),
(2, N'skripka',      N'2222', N'Смирнов',   N'Олег',   N'Викторович'),
(2, N'gitara',       N'3333', N'Козлов',    N'Дмитрий',N'Андреевич'),
(2, N'vokal',        N'4444', N'Петрова',   N'Мария',  N'Игоревна'),
(3, N'student',      N'1234', N'Иванов',    N'Иван',   N'Иванович'),
(3, N'petrov',       N'1234', N'Петров',    N'Петр',   N'Сергеевич'),
(3, N'sidorova',     N'1234', N'Сидорова',  N'Анна',   N'Владимировна'),
(3, N'smirnov_ya',   N'1234', N'Смирнов',   N'Ярослав',N'Дмитриевич');
GO

INSERT INTO lessons (category_id, title, description, image_path, base_price) VALUES
(1, N'Фортепиано для начинающих', N'Изучение нотной грамоты, постановка рук, базовые аккорды', N'fortepiano.png', 1500.00),
(2, N'Скрипка',                  N'Постановка рук, работа со смычком, базовые упражнения',       N'skripka.png',    2000.00),
(3, N'Акустическая гитара',      N'Основы боя и перебора, аккорды, простые песни',               N'gitara.png',     1200.00),
(4, N'Академический вокал',      N'Развитие голоса, дыхания, постановка звука',                  N'vokal.png',      1600.00);
GO

INSERT INTO lesson_variants (lesson_id, teacher_id, variant_name, available_seats) VALUES
(1, 2, N'Утро (9:00-10:00)',     5),
(1, 2, N'Вечер (18:00-19:00)',   3),
(2, 3, N'Утро (10:00-11:00)',    4),
(2, 3, N'Вечер (17:00-18:00)',   2),
(3, 4, N'День (14:00-15:00)',    8),
(3, 4, N'Вечер (19:00-20:00)',   1),
(4, 5, N'Утро (11:00-12:00)',    6),
(4, 5, N'Вечер (20:00-21:00)',   3);
GO