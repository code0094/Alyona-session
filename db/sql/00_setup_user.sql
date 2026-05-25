CREATE USER IF NOT EXISTS 'course_user'@'localhost' IDENTIFIED BY 'course_password';
CREATE USER IF NOT EXISTS 'course_user'@'127.0.0.1' IDENTIFIED BY 'course_password';

GRANT SELECT, INSERT, UPDATE, DELETE, CREATE, DROP, INDEX, ALTER
ON coursework_db.* TO 'course_user'@'localhost';

GRANT SELECT, INSERT, UPDATE, DELETE, CREATE, DROP, INDEX, ALTER
ON coursework_db.* TO 'course_user'@'127.0.0.1';

FLUSH PRIVILEGES;
