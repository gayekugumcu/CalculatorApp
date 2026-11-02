namespace CalculatorApp;
public partial class ScientificPage : ContentPage
{
    // Hesap makinesindeki işlemleri takip etmek için kullanılan değişkenler
    private bool isNewNumber = true;
    private double firstNumber = 0;
    private string selectedOperator = "";
    private bool hasPendingOperation = false;
    private string expression = "";

    public ScientificPage()
    {
        InitializeComponent();
    }

    // Rakam butonlarına basıldığında ekrandaki sayıyı günceller
    private void NumberButton_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        string number = button.Text;

        // Yeni sayı giriliyorsa ekranı değiştirir değilse rakamı sona ekler
        if (isNewNumber || DisplayLabel.Text == "0")
        {
            DisplayLabel.Text = number;
            isNewNumber = false;
        }
        else
        {
            DisplayLabel.Text += number;
        }

        // Devam eden bir işlem varsa girilen rakamı işleme de ekler
        if (!string.IsNullOrEmpty(expression))
        {
            expression += number;
            HistoryLabel.Text = expression;
        }

    }

    // Virgül ekler, aynı sayıya ikinci virgülü koymaz.
    private void DecimalButton_Clicked(object sender, EventArgs e)
    {
        if (isNewNumber)
        {
            DisplayLabel.Text = "0,";
            isNewNumber = false;
        }
        else if (!DisplayLabel.Text.Contains(","))
        {
            DisplayLabel.Text += ",";
        }
        else
        {
            return;
        }

        if (!string.IsNullOrEmpty(expression))
        {
            expression += ",";
            HistoryLabel.Text = expression;
        }
    }

    // Bütün hesaplamayı temizler.
    private void ClearButton_Clicked(object sender, EventArgs e)
    {
        DisplayLabel.Text = "0";
        HistoryLabel.Text = "";

        firstNumber = 0;
        selectedOperator = "";
        hasPendingOperation = false;
        isNewNumber = true;
        expression = "";
    }

    // İşleme açılış parantezi ekler.
    private void OpenParenthesisButton_Clicked(object sender, EventArgs e)
    {
        if (hasPendingOperation && expression == "")
        {
            expression = firstNumber.ToString() + selectedOperator;
            hasPendingOperation = false;
            selectedOperator = "";
        }

        expression += "(";

        HistoryLabel.Text = expression;
        isNewNumber = true;
    }

    // Açık parantez varsa kapatır.
    private void CloseParenthesisButton_Clicked(object sender, EventArgs e)
    {
        int openCount = expression.Count(c => c == '(');
        int closeCount = expression.Count(c => c == ')');

        if (openCount <= closeCount)
            return;

        expression += ")";

        HistoryLabel.Text = expression;
        isNewNumber = true;
    }

    // O anda girilen sayıyı temizler.
    private void ClearEntryButton_Clicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(expression))
        {
            int index = expression.Length - 1;

            while (index >= 0 && char.IsDigit(expression[index]))
            {
                index--;
            }

            if (index >= 0 && (expression[index] == ',' || expression[index] == '.'))
            {
                index--;

                while (index >= 0 && char.IsDigit(expression[index]))
                {
                    index--;
                }
            }

            expression = expression.Substring(0, index + 1);
            HistoryLabel.Text = expression;
        }

        DisplayLabel.Text = "0";
        isNewNumber = true;
    }

    // Son girilen karakteri siler.
    private void BackspaceButton_Clicked(object sender, EventArgs e)
    {

        if (!string.IsNullOrEmpty(expression))
        {
            expression = expression.Substring(0, expression.Length - 1);
            HistoryLabel.Text = expression;
        }

        if (isNewNumber)
            return;

        string currentNumber = DisplayLabel.Text;

        if (currentNumber.Length <= 1 ||
            (currentNumber.Length == 2 && currentNumber.StartsWith("-")))
        {
            DisplayLabel.Text = "0";
            isNewNumber = true;
        }
        else
        {
            DisplayLabel.Text = currentNumber[..^1];
        }
    }

    // Seçilen işlem işaretini kaydeder.
    private void OperatorButton_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;


        if (button.Text == "+" ||
            button.Text == "−" ||
            button.Text == "×" ||
            button.Text == "÷" ||
            button.Text == "xʸ")

        {
            if (string.IsNullOrEmpty(expression))
            {
                expression = DisplayLabel.Text;
            }

            expression += button.Text;
            HistoryLabel.Text = expression;
            isNewNumber = true;
            hasPendingOperation = false;
            selectedOperator = "";
            return;
        }

        if (!string.IsNullOrEmpty(expression))
        {
            expression += button.Text;
            HistoryLabel.Text = expression;
            isNewNumber = true;
            return;
        }

        if (!double.TryParse(DisplayLabel.Text, out double number))
            return;

        if (hasPendingOperation && !isNewNumber)
        {
            switch (selectedOperator)
            {
                case "+":
                    number = firstNumber + number;
                    break;

                case "−":
                    number = firstNumber - number;
                    break;

                case "×":
                    number = firstNumber * number;
                    break;

                case "÷":
                    if (number == 0)
                    {
                        DisplayLabel.Text = "Sıfıra bölünemez";
                        HistoryLabel.Text = "";
                        hasPendingOperation = false;
                        selectedOperator = "";
                        isNewNumber = true;
                        return;
                    }

                    number = firstNumber / number;
                    break;

                case "xʸ":
                    number = Math.Pow(firstNumber, number);
                    break;
            }

            DisplayLabel.Text = number.ToString();
        }

        firstNumber = number;
        selectedOperator = button.Text;
        hasPendingOperation = true;
        isNewNumber = true;

        HistoryLabel.Text = $"{DisplayLabel.Text} {selectedOperator}";
    }

    // İşlemi hesaplar ve sonucu gösterir.
    private void EqualButton_Clicked(object sender, EventArgs e)
    {

        if (!string.IsNullOrWhiteSpace(expression))
        {
            try
            {
                double expressionResult = CalculateExpression(expression);
                if (double.IsInfinity(expressionResult) || double.IsNaN(expressionResult))
                {
                    DisplayLabel.Text = "Sıfıra bölünemez";
                    HistoryLabel.Text = "";

                    expression = "";
                    hasPendingOperation = false;
                    selectedOperator = "";
                    isNewNumber = true;
                    return;
                }

                HistoryLabel.Text = expression + " =";
                DisplayLabel.Text = expressionResult.ToString();

                expression = "";
                hasPendingOperation = false;
                selectedOperator = "";
                isNewNumber = true;
            }
            catch
            {
                DisplayLabel.Text = "Geçersiz ifade";
                HistoryLabel.Text = "";

                expression = "";
                hasPendingOperation = false;
                selectedOperator = "";
                isNewNumber = true;
            }

            return;
        }

        if (!hasPendingOperation)
            return;

        if (!double.TryParse(DisplayLabel.Text, out double secondNumber))
            return;

        double result;

        switch (selectedOperator)
        {
            case "+":
                result = firstNumber + secondNumber;
                break;

            case "−":
                result = firstNumber - secondNumber;
                break;

            case "×":
                result = firstNumber * secondNumber;
                break;

            case "÷":
                if (secondNumber == 0)
                {
                    DisplayLabel.Text = "Sıfıra bölünemez";
                    HistoryLabel.Text = "";
                    hasPendingOperation = false;
                    selectedOperator = "";
                    isNewNumber = true;
                    return;
                }

                result = firstNumber / secondNumber;
                break;

            case "xʸ":
                result = Math.Pow(firstNumber, secondNumber);
                break;

            default:
                return;
        }

        HistoryLabel.Text = $"{firstNumber} {selectedOperator} {secondNumber} =";
        DisplayLabel.Text = result.ToString();

        isNewNumber = true;
        hasPendingOperation = false;
        selectedOperator = "";
    }

    // Sinüs hesabını dereceye göre yapar.
    private void SinButton_Clicked(object sender, EventArgs e)
    {
        if (!double.TryParse(DisplayLabel.Text, out double number))
            return;

        double radians = number * Math.PI / 180;
        double result = Math.Sin(radians);

        if (Math.Abs(result) < 1e-12)
            result = 0;

        UpdateExpressionWithResult(number, result);

        if (string.IsNullOrEmpty(expression))
            HistoryLabel.Text = $"sin({DisplayLabel.Text})";

        DisplayLabel.Text = result.ToString();
        isNewNumber = true;
    }

    // Kosinüs hesabını dereceye göre yapar.
    private void CosButton_Clicked(object sender, EventArgs e)
    {
        if (!double.TryParse(DisplayLabel.Text, out double number))
            return;

        double radians = number * Math.PI / 180;
        double result = Math.Cos(radians);

        if (Math.Abs(result) < 1e-12)
            result = 0;

        UpdateExpressionWithResult(number, result);

        if (string.IsNullOrEmpty(expression))
            HistoryLabel.Text = $"cos({DisplayLabel.Text})";

        DisplayLabel.Text = result.ToString();
        isNewNumber = true;
    }

    // Tanjantı hesaplar, tanımsız değerleri kontrol eder.
    private void TanButton_Clicked(object sender, EventArgs e)
    {
        if (!double.TryParse(DisplayLabel.Text, out double number))
            return;

        double radians = number * Math.PI / 180;

        if (Math.Abs(Math.Cos(radians)) < 1e-12)
        {
            DisplayLabel.Text = "Tanımsız sonuç";
            HistoryLabel.Text = "";
            expression = "";
            isNewNumber = true;
            return;
        }

        double result = Math.Tan(radians);

        if (Math.Abs(result) < 1e-12)
            result = 0;

        UpdateExpressionWithResult(number, result);

        if (string.IsNullOrEmpty(expression))
            HistoryLabel.Text = $"tan({DisplayLabel.Text})";

        DisplayLabel.Text = result.ToString();
        isNewNumber = true;
    }

    // 10 tabanında logaritma hesaplar.
    private void LogButton_Clicked(object sender, EventArgs e)
    {
        if (!double.TryParse(DisplayLabel.Text, out double number))
            return;

        if (number <= 0)
        {
            DisplayLabel.Text = "Geçersiz giriş";
            HistoryLabel.Text = "";
            expression = "";
            isNewNumber = true;
            return;
        }

        double result = Math.Log10(number);

        UpdateExpressionWithResult(number, result);

        if (string.IsNullOrEmpty(expression))
            HistoryLabel.Text = $"log({DisplayLabel.Text})";

        DisplayLabel.Text = result.ToString();
        isNewNumber = true;
    }

    // Doğal logaritmayı hesaplar.
    private void LnButton_Clicked(object sender, EventArgs e)
    {
        if (!double.TryParse(DisplayLabel.Text, out double number))
            return;

        if (number <= 0)
        {
            DisplayLabel.Text = "Geçersiz giriş";
            HistoryLabel.Text = "";
            expression = "";
            isNewNumber = true;
            return;
        }

        double result = Math.Log(number);

        UpdateExpressionWithResult(number, result);

        if (string.IsNullOrEmpty(expression))
            HistoryLabel.Text = $"ln({DisplayLabel.Text})";

        DisplayLabel.Text = result.ToString();
        isNewNumber = true;
    }

    // Pi sayısını ekrana ve işleme ekler.
    private void PiButton_Clicked(object sender, EventArgs e)
    {
        string piValue = Math.PI.ToString(
            System.Globalization.CultureInfo.InvariantCulture);

        DisplayLabel.Text = Math.PI.ToString();

        if (!string.IsNullOrEmpty(expression))
        {
            expression += "(" + piValue + ")";
            HistoryLabel.Text = expression;
        }

        isNewNumber = false;
    }

    // e sayısını ekrana ve işleme ekler.
    private void EButton_Clicked(object sender, EventArgs e)
    {
        string eValue = Math.E.ToString(
            System.Globalization.CultureInfo.InvariantCulture);

        DisplayLabel.Text = Math.E.ToString();

        if (!string.IsNullOrEmpty(expression))
        {
            expression += "(" + eValue + ")";
            HistoryLabel.Text = expression;
        }

        isNewNumber = false;
    }

    // Faktöriyel hesabını yapar.
    private void FactorialButton_Clicked(object sender, EventArgs e)
    {
        if (!double.TryParse(DisplayLabel.Text, out double number))
            return;

        if (number < 0 || number != Math.Floor(number) || number > 170)
        {
            DisplayLabel.Text = "Geçersiz giriş";
            HistoryLabel.Text = "";
            isNewNumber = true;
            return;
        }

        double result = 1;

        for (int i = 2; i <= (int)number; i++)
        {
            result *= i;
        }

        UpdateExpressionWithResult(number, result);

        if (string.IsNullOrEmpty(expression))
            HistoryLabel.Text = $"fact({DisplayLabel.Text})";

        DisplayLabel.Text = result.ToString();
        isNewNumber = true;
    }

    // Sayının karesini alır.
    private void SquareButton_Clicked(object sender, EventArgs e)
    {
        if (!double.TryParse(DisplayLabel.Text, out double number))
            return;

        double result = number * number;

        UpdateExpressionWithResult(number, result);

        if (string.IsNullOrEmpty(expression))
            HistoryLabel.Text = $"sqr({DisplayLabel.Text})";

        DisplayLabel.Text = result.ToString();
        isNewNumber = true;
    }

    // Karekök alır, negatif sayıları kabul etmez.
    private void SqrtButton_Clicked(object sender, EventArgs e)
    {
        if (!double.TryParse(DisplayLabel.Text, out double number))
            return;

        if (number < 0)
        {
            DisplayLabel.Text = "Geçersiz giriş";
            HistoryLabel.Text = "";
            expression = "";
            isNewNumber = true;
            return;
        }

        double result = Math.Sqrt(number);

        UpdateExpressionWithResult(number, result);

        if (string.IsNullOrEmpty(expression))
            HistoryLabel.Text = $"√({DisplayLabel.Text})";

        DisplayLabel.Text = result.ToString();
        isNewNumber = true;
    }

    // Sayının tersini hesaplar.
    private void ReciprocalButton_Clicked(object sender, EventArgs e)
    {
        if (!double.TryParse(DisplayLabel.Text, out double number))
            return;

        if (number == 0)
        {
            DisplayLabel.Text = "Sıfıra bölünemez";
            HistoryLabel.Text = "";
            expression = "";
            isNewNumber = true;
            return;
        }

        double result = 1 / number;

        UpdateExpressionWithResult(number, result);

        if (string.IsNullOrEmpty(expression))
            HistoryLabel.Text = $"1/({DisplayLabel.Text})";

        DisplayLabel.Text = result.ToString();
        isNewNumber = true;
    }

    // Sayının işaretini değiştirir.
    private void SignButton_Clicked(object sender, EventArgs e)
    {
        if (!double.TryParse(DisplayLabel.Text, out double number))
            return;

        string oldNumber = DisplayLabel.Text;
        number = -number;

        string newNumber = number.ToString();

        DisplayLabel.Text = newNumber;

        if (!string.IsNullOrEmpty(expression))
        {
            string oldPart = "(" + oldNumber + ")";

            if (expression.EndsWith(oldPart))
            {
                expression = expression.Substring(
                    0, expression.Length - oldPart.Length
                ) + "(" + newNumber + ")";
            }
            else if (expression.EndsWith(oldNumber))
            {
                expression = expression.Substring(
                    0, expression.Length - oldNumber.Length
                ) + "(" + newNumber + ")";
            }

            HistoryLabel.Text = expression;
        }

        isNewNumber = false;
    }

    // Sayıyı yüzdeye çevirir.
    private void PercentButton_Clicked(object sender, EventArgs e)
    {
        if (!double.TryParse(DisplayLabel.Text, out double number))
            return;

        double result = number / 100;

        UpdateExpressionWithResult(number, result);

        if (string.IsNullOrEmpty(expression))
            HistoryLabel.Text = $"{DisplayLabel.Text}%";

        DisplayLabel.Text = result.ToString();
        isNewNumber = true;
    }

    // Parantezli üs işlemlerini hesaplar.
    private string CalculatePowerParentheses(string input)
    {
        string pattern = @"\(([^()]+)\)xʸ(\d+(?:\.\d+)?)";

        while (System.Text.RegularExpressions.Regex.IsMatch(input, pattern))
        {
            input = System.Text.RegularExpressions.Regex.Replace(
                input,
                pattern,
                match =>
                {
                    double baseNumber = CalculateExpression(
                        match.Groups[1].Value);

                    double exponent = double.Parse(
                        match.Groups[2].Value,
                        System.Globalization.CultureInfo.InvariantCulture);

                    return Math.Pow(baseNumber, exponent).ToString(
                        System.Globalization.CultureInfo.InvariantCulture);
                });
        }

        return input;
    }

    // İfadeyi hesaplamaya uygun hale getirir.
    private double CalculateExpression(string input)
    {
        string mathExpression = input
            .Replace("×", "*")
            .Replace("÷", "/")
            .Replace("−", "-")
            .Replace(",", ".");

        mathExpression = CalculatePowerParentheses(mathExpression);
        string numberPattern = @"(?:\d+(?:\.\d+)?|\.\d+)";

        string powerPattern =
            $@"(?<base>{numberPattern})xʸ(?<exponent>{numberPattern})";

        while (System.Text.RegularExpressions.Regex.IsMatch(
            mathExpression, powerPattern))
        {
            mathExpression = System.Text.RegularExpressions.Regex.Replace(
                mathExpression,
                powerPattern,
                match =>
                {
                    double baseNumber = double.Parse(
                        match.Groups["base"].Value,
                        System.Globalization.CultureInfo.InvariantCulture);

                    double exponent = double.Parse(
                        match.Groups["exponent"].Value,
                        System.Globalization.CultureInfo.InvariantCulture);

                    double powerResult = Math.Pow(baseNumber, exponent);

                    return powerResult.ToString(
                        System.Globalization.CultureInfo.InvariantCulture);
                });
        }

        var table = new System.Data.DataTable();

        object result = table.Compute(mathExpression, "");

        return Convert.ToDouble(result);
    }

    // İşlemdeki eski sayıyı yeni sonuçla değiştirir.
    private void UpdateExpressionWithResult(double oldNumber, double result)
    {
        if (string.IsNullOrEmpty(expression))
            return;

        string oldText = oldNumber.ToString();
        string newText = result.ToString();

        string oldPart = "(" + oldText + ")";

        if (expression.EndsWith(oldPart))
        {
            expression = expression.Substring(
                0, expression.Length - oldPart.Length
            ) + "(" + newText + ")";
        }
        else if (expression.EndsWith(oldText))
        {
            expression = expression.Substring(
                0, expression.Length - oldText.Length
            ) + "(" + newText + ")";
        }

        HistoryLabel.Text = expression;
    }
}