USE coursework_db;

-- 1. Ноутбуки, которые поместятся в сумку шириной 0,5 м, глубиной 0,3 м, высотой 0,4 м.
SELECT name, model, width_m, depth_m, height_m
FROM laptops
WHERE width_m <= 0.50
  AND depth_m <= 0.30
  AND height_m <= 0.40;

-- 2. Модели по требованиям: Intel-P4 2,4Ггц, ОЗУ 256 Мб, HDD 100 Гб, DVD-CD-RW, 14", Windows XP.
SELECT name, model, processor, ram_mb, hdd_gb, drive_type, monitor_inches, operating_system
FROM laptops
WHERE processor = 'Intel-P4 2,4Ггц'
  AND ram_mb = 256
  AND hdd_gb = 100
  AND drive_type = 'DVD-CD-RW'
  AND monitor_inches = 14.0
  AND operating_system = 'Windows XP';

-- 3. Поставщики из Лондона, которые поставляют мобильные модели.
SELECT DISTINCT s.supplier_name, s.city, s.address
FROM suppliers s
JOIN deliveries d ON d.supplier_id = s.supplier_id
JOIN laptops l ON l.laptop_id = d.laptop_id
JOIN laptop_types lt ON lt.type_id = l.type_id
WHERE s.city = 'Лондон'
  AND lt.type_name = 'мобильная модель';

-- 4. Общая стоимость каждой поставки.
SELECT d.delivery_id, s.supplier_name, l.name AS laptop_name, d.delivery_date,
       d.quantity, d.unit_price, d.quantity * d.unit_price AS delivery_total
FROM deliveries d
JOIN suppliers s ON s.supplier_id = d.supplier_id
JOIN laptops l ON l.laptop_id = d.laptop_id
ORDER BY d.delivery_id;

-- 5. Все ноутбуки, поставляемые поставщиком RoverBook.
SELECT l.name, l.model, lt.type_name, d.delivery_date, d.quantity, d.unit_price
FROM deliveries d
JOIN suppliers s ON s.supplier_id = d.supplier_id
JOIN laptops l ON l.laptop_id = d.laptop_id
JOIN laptop_types lt ON lt.type_id = l.type_id
WHERE s.supplier_name = 'RoverBook';

-- 6. Все ноутбуки и их количество, поставляемые поставщиком SComp.
SELECT l.name, l.model, SUM(d.quantity) AS total_quantity
FROM deliveries d
JOIN suppliers s ON s.supplier_id = d.supplier_id
JOIN laptops l ON l.laptop_id = d.laptop_id
WHERE s.supplier_name = 'SComp'
GROUP BY l.laptop_id, l.name, l.model;

-- 7. Количество и общая стоимость всех ноутбуков, закупленных в мае 2004 года.
SELECT SUM(quantity) AS total_quantity,
       SUM(quantity * unit_price) AS total_sum
FROM deliveries
WHERE delivery_date >= '2004-05-01'
  AND delivery_date < '2004-06-01';

-- 8. Общее количество каждой поставленной марки ноутбука для каждого поставщика.
SELECT s.supplier_name, l.name AS laptop_name, SUM(d.quantity) AS total_quantity
FROM deliveries d
JOIN suppliers s ON s.supplier_id = d.supplier_id
JOIN laptops l ON l.laptop_id = d.laptop_id
GROUP BY s.supplier_name, l.name
ORDER BY s.supplier_name, l.name;

-- 9. Поставщики, не поставляющие экономическую модель.
SELECT s.supplier_name, s.city, s.address
FROM suppliers s
WHERE NOT EXISTS (
    SELECT 1
    FROM deliveries d
    JOIN laptops l ON l.laptop_id = d.laptop_id
    JOIN laptop_types lt ON lt.type_id = l.type_id
    WHERE d.supplier_id = s.supplier_id
      AND lt.type_name = 'экономическая модель'
);

-- 10. Дата самой дорогой поставки и данные поставщика.
SELECT d.delivery_id, d.delivery_date, s.supplier_name, s.city, s.address,
       d.quantity * d.unit_price AS delivery_total
FROM deliveries d
JOIN suppliers s ON s.supplier_id = d.supplier_id
WHERE d.quantity * d.unit_price = (
    SELECT MAX(quantity * unit_price)
    FROM deliveries
);

-- 11. Для каждого поставщика дата самой дешевой поставки.
SELECT s.supplier_name, d.delivery_id, d.delivery_date,
       d.quantity * d.unit_price AS delivery_total
FROM suppliers s
JOIN deliveries d ON d.supplier_id = s.supplier_id
WHERE d.quantity * d.unit_price = (
    SELECT MIN(d2.quantity * d2.unit_price)
    FROM deliveries d2
    WHERE d2.supplier_id = s.supplier_id
)
ORDER BY s.supplier_name;

-- 12. Поставщики, которые не поставляют Rover1000 или TochBook20.
SELECT s.supplier_name, s.city, s.address
FROM suppliers s
WHERE NOT EXISTS (
    SELECT 1
    FROM deliveries d
    JOIN laptops l ON l.laptop_id = d.laptop_id
    WHERE d.supplier_id = s.supplier_id
      AND l.name IN ('Rover1000', 'TochBook20')
);

-- 13. UPDATE: изменить объем оперативной памяти ноутбука Rover1000.
UPDATE laptops
SET ram_mb = 512
WHERE name = 'Rover1000';

-- 14. DELETE: удалить из списка поставщиков фирму SComp.
DELETE FROM deliveries
WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE supplier_name = 'SComp');

DELETE FROM suppliers
WHERE supplier_name = 'SComp';
