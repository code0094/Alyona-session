USE coursework_db;

INSERT INTO laptop_types (type_name, description) VALUES
('Nautilus', 'Флагманская серия ноутбуков'),
('экономическая модель', 'Доступные ноутбуки для офиса и учебы'),
('мини', 'Компактные ноутбуки для поездок'),
('мобильная модель', 'Легкие ноутбуки с длительной автономностью'),
('производительная модель', 'Мощные ноутбуки для сложных задач');

INSERT INTO laptops
(type_id, name, model, width_m, depth_m, height_m, processor, ram_mb, hdd_gb, drive_type, monitor_inches, operating_system, unit_cost)
VALUES
((SELECT type_id FROM laptop_types WHERE type_name = 'Nautilus'), 'Nautilus Pro 15', 'Nautilus-P15', 0.36, 0.25, 0.03, 'Intel-P4 2.8Ггц', 512, 120, 'DVD-CD-RW', 15.0, 'Windows XP', 31000.00),
((SELECT type_id FROM laptop_types WHERE type_name = 'экономическая модель'), 'Rover1000', 'Rover1000', 0.39, 0.28, 0.04, 'Intel-Celeron 2.0Ггц', 256, 80, 'DVD', 14.0, 'Windows XP', 18500.00),
((SELECT type_id FROM laptop_types WHERE type_name = 'мини'), 'MiniBook Air 12', 'MBA-12', 0.28, 0.20, 0.03, 'Intel-P3 1.6Ггц', 256, 60, 'нет', 12.1, 'Windows XP', 20500.00),
((SELECT type_id FROM laptop_types WHERE type_name = 'мобильная модель'), 'TochBook20', 'TochBook20', 0.34, 0.24, 0.03, 'Intel-P4 2,4Ггц', 256, 100, 'DVD-CD-RW', 14.0, 'Windows XP', 27500.00),
((SELECT type_id FROM laptop_types WHERE type_name = 'мобильная модель'), 'TravelMate London', 'TML-14', 0.33, 0.23, 0.03, 'Intel-P4 2,4Ггц', 512, 100, 'DVD-CD-RW', 14.0, 'Windows XP', 28500.00),
((SELECT type_id FROM laptop_types WHERE type_name = 'производительная модель'), 'PowerNote X', 'PN-X', 0.42, 0.31, 0.05, 'Intel-P4 3.2Ггц', 1024, 160, 'DVD-RW', 17.0, 'Windows XP Professional', 39500.00),
((SELECT type_id FROM laptop_types WHERE type_name = 'экономическая модель'), 'OfficeStart 14', 'OS-14', 0.37, 0.26, 0.04, 'Intel-P4 2,4Ггц', 256, 100, 'DVD-CD-RW', 14.0, 'Windows XP', 21000.00),
((SELECT type_id FROM laptop_types WHERE type_name = 'мини'), 'Compact XP', 'CXP-11', 0.27, 0.19, 0.03, 'Intel-Pentium M 1.4Ггц', 256, 40, 'нет', 11.6, 'Windows XP', 19000.00);

INSERT INTO suppliers (supplier_name, city, address, phone) VALUES
('RoverBook', 'Лондон', 'Baker Street, 18', '+44-20-1000-1000'),
('SComp', 'Москва', 'ул. Тверская, 7', '+7-495-111-22-33'),
('TechLine', 'Лондон', 'Oxford Street, 45', '+44-20-2000-2000'),
('CompMarket', 'Нижний Новгород', 'ул. Большая Покровская, 15', '+7-831-222-33-44'),
('NotebookTrade', 'Берлин', 'Alexanderplatz, 3', '+49-30-3333-3333');

INSERT INTO deliveries (supplier_id, laptop_id, delivery_date, quantity, unit_price) VALUES
((SELECT supplier_id FROM suppliers WHERE supplier_name = 'RoverBook'), (SELECT laptop_id FROM laptops WHERE name = 'Rover1000'), '2004-05-03', 15, 24500.00),
((SELECT supplier_id FROM suppliers WHERE supplier_name = 'RoverBook'), (SELECT laptop_id FROM laptops WHERE name = 'TochBook20'), '2004-05-12', 8, 34000.00),
((SELECT supplier_id FROM suppliers WHERE supplier_name = 'SComp'), (SELECT laptop_id FROM laptops WHERE name = 'OfficeStart 14'), '2004-05-18', 20, 26000.00),
((SELECT supplier_id FROM suppliers WHERE supplier_name = 'SComp'), (SELECT laptop_id FROM laptops WHERE name = 'Compact XP'), '2004-04-20', 12, 23500.00),
((SELECT supplier_id FROM suppliers WHERE supplier_name = 'TechLine'), (SELECT laptop_id FROM laptops WHERE name = 'TravelMate London'), '2004-05-25', 10, 36000.00),
((SELECT supplier_id FROM suppliers WHERE supplier_name = 'TechLine'), (SELECT laptop_id FROM laptops WHERE name = 'Nautilus Pro 15'), '2004-06-01', 5, 42000.00),
((SELECT supplier_id FROM suppliers WHERE supplier_name = 'CompMarket'), (SELECT laptop_id FROM laptops WHERE name = 'PowerNote X'), '2004-05-28', 6, 52000.00),
((SELECT supplier_id FROM suppliers WHERE supplier_name = 'CompMarket'), (SELECT laptop_id FROM laptops WHERE name = 'Rover1000'), '2004-03-15', 9, 23800.00),
((SELECT supplier_id FROM suppliers WHERE supplier_name = 'NotebookTrade'), (SELECT laptop_id FROM laptops WHERE name = 'MiniBook Air 12'), '2004-05-08', 7, 25500.00),
((SELECT supplier_id FROM suppliers WHERE supplier_name = 'NotebookTrade'), (SELECT laptop_id FROM laptops WHERE name = 'PowerNote X'), '2004-02-10', 4, 51000.00);
