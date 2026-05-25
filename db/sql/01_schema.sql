DROP DATABASE IF EXISTS coursework_db;
CREATE DATABASE coursework_db
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE coursework_db;

CREATE TABLE laptop_types (
    type_id INT AUTO_INCREMENT PRIMARY KEY,
    type_name VARCHAR(80) NOT NULL UNIQUE,
    description VARCHAR(255) NULL
);

CREATE TABLE laptops (
    laptop_id INT AUTO_INCREMENT PRIMARY KEY,
    type_id INT NOT NULL,
    name VARCHAR(100) NOT NULL,
    model VARCHAR(100) NOT NULL,
    width_m DECIMAL(5,2) NOT NULL,
    depth_m DECIMAL(5,2) NOT NULL,
    height_m DECIMAL(5,2) NOT NULL,
    processor VARCHAR(100) NOT NULL,
    ram_mb INT NOT NULL,
    hdd_gb INT NOT NULL,
    drive_type VARCHAR(60) NOT NULL,
    monitor_inches DECIMAL(4,1) NOT NULL,
    operating_system VARCHAR(80) NOT NULL,
    unit_cost DECIMAL(10,2) NOT NULL,
    CONSTRAINT uq_laptops_name UNIQUE (name),
    CONSTRAINT fk_laptops_type FOREIGN KEY (type_id)
        REFERENCES laptop_types(type_id)
        ON UPDATE CASCADE
        ON DELETE RESTRICT
);

CREATE TABLE suppliers (
    supplier_id INT AUTO_INCREMENT PRIMARY KEY,
    supplier_name VARCHAR(120) NOT NULL UNIQUE,
    city VARCHAR(80) NOT NULL,
    address VARCHAR(255) NOT NULL,
    phone VARCHAR(40) NULL
);

CREATE TABLE deliveries (
    delivery_id INT AUTO_INCREMENT PRIMARY KEY,
    supplier_id INT NOT NULL,
    laptop_id INT NOT NULL,
    delivery_date DATE NOT NULL,
    quantity INT NOT NULL,
    unit_price DECIMAL(10,2) NOT NULL,
    CONSTRAINT fk_deliveries_supplier FOREIGN KEY (supplier_id)
        REFERENCES suppliers(supplier_id)
        ON UPDATE CASCADE
        ON DELETE RESTRICT,
    CONSTRAINT fk_deliveries_laptop FOREIGN KEY (laptop_id)
        REFERENCES laptops(laptop_id)
        ON UPDATE CASCADE
        ON DELETE RESTRICT
);
