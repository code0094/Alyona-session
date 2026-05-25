namespace LaptopSupplyApp;

using System.Data;
using MySqlConnector;

public partial class Form1 : Form
{
    private readonly TextBox serverBox = new() { Text = "127.0.0.1" };
    private readonly TextBox portBox = new() { Text = "3306" };
    private readonly TextBox databaseBox = new() { Text = "coursework_db" };
    private readonly TextBox userBox = new() { Text = "course_user" };
    private readonly TextBox passwordBox = new() { Text = "course_password", UseSystemPasswordChar = true };
    private readonly Label statusLabel = new() { AutoSize = true, Text = "Не подключено" };
    private readonly ComboBox queryBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DataGridView dataGrid = new() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
    private readonly ComboBox laptopBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox supplierBox = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker deliveryDatePicker = new() { Format = DateTimePickerFormat.Short };
    private readonly NumericUpDown quantityBox = new() { Minimum = 1, Maximum = 10000, Value = 1 };
    private readonly NumericUpDown priceBox = new() { Minimum = 1, Maximum = 1000000, DecimalPlaces = 2, Increment = 100, Value = 10000 };
    private readonly Dictionary<string, string> queryMap;

    public Form1()
    {
        queryMap = BuildQueryMap();
        BuildUi();
        Load += async (_, _) => await ConnectAndLoadAsync();
    }

    private string ConnectionString => new MySqlConnectionStringBuilder
    {
        Server = serverBox.Text.Trim(),
        Port = uint.TryParse(portBox.Text.Trim(), out var port) ? port : 3306,
        Database = databaseBox.Text.Trim(),
        UserID = userBox.Text.Trim(),
        Password = passwordBox.Text,
        CharacterSet = "utf8mb4"
    }.ConnectionString;

    private void BuildUi()
    {
        Text = "Курсовая БД: поставки ноутбуков";
        Width = 1180;
        Height = 760;
        MinimumSize = new Size(980, 620);
        StartPosition = FormStartPosition.CenterScreen;

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 4, ColumnCount = 1, Padding = new Padding(10) };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        root.Controls.Add(BuildConnectionPanel(), 0, 0);
        root.Controls.Add(BuildQueryPanel(), 0, 1);
        root.Controls.Add(dataGrid, 0, 2);
        root.Controls.Add(BuildDeliveryPanel(), 0, 3);
        Controls.Add(root);
    }

    private Control BuildConnectionPanel()
    {
        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
        panel.Controls.AddRange(new Control[]
        {
            Label("Сервер"), Sized(serverBox, 120),
            Label("Порт"), Sized(portBox, 60),
            Label("База"), Sized(databaseBox, 130),
            Label("Пользователь"), Sized(userBox, 120),
            Label("Пароль"), Sized(passwordBox, 120),
            Button("Подключиться", async (_, _) => await ConnectAndLoadAsync()),
            statusLabel
        });
        return panel;
    }

    private Control BuildQueryPanel()
    {
        foreach (var key in queryMap.Keys)
        {
            queryBox.Items.Add(key);
        }
        queryBox.SelectedIndex = 0;

        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true, Padding = new Padding(0, 8, 0, 8) };
        panel.Controls.AddRange(new Control[]
        {
            Label("Запрос"),
            Sized(queryBox, 520),
            Button("Выполнить", async (_, _) => await RunSelectedQueryAsync()),
            Button("Ноутбуки", async (_, _) => await LoadTableAsync("laptops")),
            Button("Поставщики", async (_, _) => await LoadTableAsync("suppliers")),
            Button("Поставки", async (_, _) => await LoadDeliveriesAsync()),
            Button("Изменить RAM Rover1000", async (_, _) => await UpdateRoverRamAsync()),
            Button("Удалить SComp", async (_, _) => await DeleteSCompAsync())
        });
        return panel;
    }

    private Control BuildDeliveryPanel()
    {
        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true, Padding = new Padding(0, 8, 0, 0) };
        panel.Controls.AddRange(new Control[]
        {
            Label("Ноутбук"), Sized(laptopBox, 190),
            Label("Поставщик"), Sized(supplierBox, 170),
            Label("Дата"), Sized(deliveryDatePicker, 110),
            Label("Кол-во"), Sized(quantityBox, 80),
            Label("Цена"), Sized(priceBox, 100),
            Button("Добавить поставку", async (_, _) => await AddDeliveryAsync()),
            Button("Удалить выбранную поставку", async (_, _) => await DeleteSelectedDeliveryAsync())
        });
        return panel;
    }

    private static Label Label(string text) => new() { Text = text, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(0, 6, 0, 0) };

    private static Control Sized(Control control, int width)
    {
        control.Width = width;
        return control;
    }

    private static Button Button(string text, EventHandler handler)
    {
        var button = new Button { Text = text, AutoSize = true, Height = 30 };
        button.Click += handler;
        return button;
    }

    private async Task ConnectAndLoadAsync()
    {
        try
        {
            await using var connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync();
            statusLabel.Text = "Подключено";
            await LoadLookupsAsync();
            await LoadDeliveriesAsync();
        }
        catch (Exception ex)
        {
            statusLabel.Text = "Ошибка подключения";
            MessageBox.Show(ex.Message, "MySQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task LoadLookupsAsync()
    {
        await FillComboAsync(laptopBox, "SELECT laptop_id, CONCAT(name, ' (', model, ')') AS title FROM laptops ORDER BY name", "title", "laptop_id");
        await FillComboAsync(supplierBox, "SELECT supplier_id, supplier_name FROM suppliers ORDER BY supplier_name", "supplier_name", "supplier_id");
    }

    private async Task FillComboAsync(ComboBox comboBox, string sql, string display, string value)
    {
        await using var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();
        using var adapter = new MySqlDataAdapter(sql, connection);
        var table = new DataTable();
        adapter.Fill(table);
        comboBox.DisplayMember = display;
        comboBox.ValueMember = value;
        comboBox.DataSource = table;
    }

    private async Task LoadTableAsync(string tableName)
    {
        await LoadQueryAsync($"SELECT * FROM {tableName};");
    }

    private async Task LoadDeliveriesAsync()
    {
        const string sql = """
            SELECT d.delivery_id, s.supplier_name, l.name AS laptop_name, d.delivery_date,
                   d.quantity, d.unit_price, d.quantity * d.unit_price AS delivery_total
            FROM deliveries d
            JOIN suppliers s ON s.supplier_id = d.supplier_id
            JOIN laptops l ON l.laptop_id = d.laptop_id
            ORDER BY d.delivery_id;
            """;
        await LoadQueryAsync(sql);
    }

    private async Task RunSelectedQueryAsync()
    {
        if (queryBox.SelectedItem is string name && queryMap.TryGetValue(name, out var sql))
        {
            await LoadQueryAsync(sql);
        }
    }

    private async Task LoadQueryAsync(string sql)
    {
        try
        {
            await using var connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync();
            using var adapter = new MySqlDataAdapter(sql, connection);
            var table = new DataTable();
            adapter.Fill(table);
            dataGrid.DataSource = table;
            statusLabel.Text = $"Строк: {table.Rows.Count}";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Запрос", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task AddDeliveryAsync()
    {
        if (laptopBox.SelectedValue is null || supplierBox.SelectedValue is null)
        {
            MessageBox.Show("Выберите ноутбук и поставщика.", "Поставка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        const string sql = """
            INSERT INTO deliveries (supplier_id, laptop_id, delivery_date, quantity, unit_price)
            VALUES (@supplier_id, @laptop_id, @delivery_date, @quantity, @unit_price);
            """;
        try
        {
            await using var connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync();
            await using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@supplier_id", supplierBox.SelectedValue);
            command.Parameters.AddWithValue("@laptop_id", laptopBox.SelectedValue);
            command.Parameters.AddWithValue("@delivery_date", deliveryDatePicker.Value.Date);
            command.Parameters.AddWithValue("@quantity", quantityBox.Value);
            command.Parameters.AddWithValue("@unit_price", priceBox.Value);
            await command.ExecuteNonQueryAsync();
            await LoadDeliveriesAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Добавление поставки", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task DeleteSelectedDeliveryAsync()
    {
        if (dataGrid.CurrentRow?.Cells["delivery_id"].Value is not object value)
        {
            MessageBox.Show("Выберите строку поставки с delivery_id.", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        const string sql = "DELETE FROM deliveries WHERE delivery_id = @delivery_id;";
        try
        {
            await using var connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync();
            await using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@delivery_id", value);
            await command.ExecuteNonQueryAsync();
            await LoadDeliveriesAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Удаление поставки", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task UpdateRoverRamAsync()
    {
        const string sql = "UPDATE laptops SET ram_mb = 512 WHERE name = 'Rover1000';";
        await ExecuteCommandAsync(sql, "ОЗУ ноутбука Rover1000 изменено на 512 Мб.");
        await LoadTableAsync("laptops");
    }

    private async Task DeleteSCompAsync()
    {
        var confirm = MessageBox.Show(
            "Будут удалены поставки SComp и сам поставщик. Продолжить?",
            "DELETE SComp",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes)
        {
            return;
        }

        try
        {
            await using var connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            await using (var deleteDeliveries = new MySqlCommand("""
                DELETE FROM deliveries
                WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE supplier_name = 'SComp');
                """, connection, transaction))
            {
                await deleteDeliveries.ExecuteNonQueryAsync();
            }

            await using (var deleteSupplier = new MySqlCommand("DELETE FROM suppliers WHERE supplier_name = 'SComp';", connection, transaction))
            {
                await deleteSupplier.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
            statusLabel.Text = "Поставщик SComp удален вместе с его поставками.";
            MessageBox.Show(statusLabel.Text, "DELETE SComp", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadDeliveriesAsync();
            await LoadLookupsAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "DELETE SComp", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task ExecuteCommandAsync(string sql, string successMessage)
    {
        try
        {
            await using var connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync();
            await using var command = new MySqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
            statusLabel.Text = successMessage;
            MessageBox.Show(successMessage, "Команда выполнена", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Команда", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static Dictionary<string, string> BuildQueryMap() => new()
    {
        ["1. Ноутбуки помещаются в сумку 0,5 x 0,3 x 0,4 м"] = """
            SELECT name, model, width_m, depth_m, height_m
            FROM laptops
            WHERE width_m <= 0.50 AND depth_m <= 0.30 AND height_m <= 0.40;
            """,
        ["2. Требования Intel-P4 2,4Ггц / 256 Мб / 100 Гб / DVD-CD-RW / 14 / Windows XP"] = """
            SELECT name, model, processor, ram_mb, hdd_gb, drive_type, monitor_inches, operating_system
            FROM laptops
            WHERE processor = 'Intel-P4 2,4Ггц'
              AND ram_mb = 256 AND hdd_gb = 100
              AND drive_type = 'DVD-CD-RW'
              AND monitor_inches = 14.0
              AND operating_system = 'Windows XP';
            """,
        ["3. Лондонские поставщики мобильных моделей"] = """
            SELECT DISTINCT s.supplier_name, s.city, s.address
            FROM suppliers s
            JOIN deliveries d ON d.supplier_id = s.supplier_id
            JOIN laptops l ON l.laptop_id = d.laptop_id
            JOIN laptop_types lt ON lt.type_id = l.type_id
            WHERE s.city = 'Лондон' AND lt.type_name = 'мобильная модель';
            """,
        ["4. Общая стоимость каждой поставки"] = """
            SELECT d.delivery_id, s.supplier_name, l.name AS laptop_name, d.delivery_date,
                   d.quantity, d.unit_price, d.quantity * d.unit_price AS delivery_total
            FROM deliveries d
            JOIN suppliers s ON s.supplier_id = d.supplier_id
            JOIN laptops l ON l.laptop_id = d.laptop_id
            ORDER BY d.delivery_id;
            """,
        ["5. Ноутбуки поставщика RoverBook"] = """
            SELECT l.name, l.model, lt.type_name, d.delivery_date, d.quantity, d.unit_price
            FROM deliveries d
            JOIN suppliers s ON s.supplier_id = d.supplier_id
            JOIN laptops l ON l.laptop_id = d.laptop_id
            JOIN laptop_types lt ON lt.type_id = l.type_id
            WHERE s.supplier_name = 'RoverBook';
            """,
        ["6. Ноутбуки и количество от SComp"] = """
            SELECT l.name, l.model, SUM(d.quantity) AS total_quantity
            FROM deliveries d
            JOIN suppliers s ON s.supplier_id = d.supplier_id
            JOIN laptops l ON l.laptop_id = d.laptop_id
            WHERE s.supplier_name = 'SComp'
            GROUP BY l.laptop_id, l.name, l.model;
            """,
        ["7. Количество и сумма закупок за май 2004"] = """
            SELECT SUM(quantity) AS total_quantity, SUM(quantity * unit_price) AS total_sum
            FROM deliveries
            WHERE delivery_date >= '2004-05-01' AND delivery_date < '2004-06-01';
            """,
        ["8. Количество каждой марки для каждого поставщика"] = """
            SELECT s.supplier_name, l.name AS laptop_name, SUM(d.quantity) AS total_quantity
            FROM deliveries d
            JOIN suppliers s ON s.supplier_id = d.supplier_id
            JOIN laptops l ON l.laptop_id = d.laptop_id
            GROUP BY s.supplier_name, l.name
            ORDER BY s.supplier_name, l.name;
            """,
        ["9. Поставщики без экономической модели"] = """
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
            """,
        ["10. Самая дорогая поставка"] = """
            SELECT d.delivery_id, d.delivery_date, s.supplier_name, s.city, s.address,
                   d.quantity * d.unit_price AS delivery_total
            FROM deliveries d
            JOIN suppliers s ON s.supplier_id = d.supplier_id
            WHERE d.quantity * d.unit_price = (SELECT MAX(quantity * unit_price) FROM deliveries);
            """,
        ["11. Самая дешевая поставка каждого поставщика"] = """
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
            """,
        ["12. Поставщики без Rover1000 или TochBook20"] = """
            SELECT s.supplier_name, s.city, s.address
            FROM suppliers s
            WHERE NOT EXISTS (
                SELECT 1
                FROM deliveries d
                JOIN laptops l ON l.laptop_id = d.laptop_id
                WHERE d.supplier_id = s.supplier_id
                  AND l.name IN ('Rover1000', 'TochBook20')
            );
            """
    };
}
