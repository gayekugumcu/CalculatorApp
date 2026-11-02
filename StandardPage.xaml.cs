namespace CalculatorApp;
public partial class StandardPage : ContentPage
{
    // Yeni sayı girilip girilmediğini kontrol eder.
    private bool isNewNumber = true;

    // İşlemdeki ilk sayıyı tutar.
    private double firstNumber = 0;

    // Seçilen işlem işaretini tutar.
    private string selectedOperator = "";

    // Bekleyen bir işlem olup olmadığını tutar.
    private bool hasPendingOperation = false;

    // Sonucun ekranda gösterilip gösterilmediğini takip eder.
    private bool isResultShown = false;

    public StandardPage()
    {
        InitializeComponent();
    }

    // Rakamları ekrana yazar.
    private void NumberButton_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        string number = button.Text;

        // Sonuçtan sonra yeni sayı girilirse eski işlem geçmişini temizler.
        if (isResultShown)
        {
            HistoryLabel.Text = "";
            isResultShown = false;
        }

        if (isNewNumber || DisplayLabel.Text == "0")
        {
            DisplayLabel.Text = number;
            isNewNumber = false;
        }
        else
        {
            DisplayLabel.Text += number;
        }
    }

    // Sayıya virgül ekler.
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
    }

    // Bütün işlemi sıfırlar.
    private void ClearButton_Clicked(object sender, EventArgs e)
    {
        DisplayLabel.Text = "0";
        HistoryLabel.Text = "";

        firstNumber = 0;
        selectedOperator = "";
        hasPendingOperation = false;
        isNewNumber = true;
        isResultShown = false;
    }

    // Sadece ekrandaki sayıyı temizler.
    private void ClearEntryButton_Clicked(object sender, EventArgs e)
    {
        DisplayLabel.Text = "0";
        isNewNumber = true;
    }

    // Sayının son basamağını siler.
    private void BackspaceButton_Clicked(object sender, EventArgs e)
    {
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

    // İşlem butonlarını kontrol eder.
    private void OperatorButton_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;

        if (!double.TryParse(DisplayLabel.Text, out double number))
            return;

        // Önceden bekleyen işlem varsa önce onu hesaplar.
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
                    // Sıfıra bölünmesini engeller.
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
            }

            DisplayLabel.Text = number.ToString();
        }

        firstNumber = number;
        selectedOperator = button.Text;
        hasPendingOperation = true;
        isNewNumber = true;

        HistoryLabel.Text = $"{DisplayLabel.Text} {selectedOperator}";
    }

    // Eşittire basılınca sonucu hesaplar.
    private void EqualButton_Clicked(object sender, EventArgs e)
    {
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

            default:
                return;
        }

        HistoryLabel.Text = $"{firstNumber} {selectedOperator} {secondNumber} =";
        DisplayLabel.Text = result.ToString();

        isNewNumber = true;
        hasPendingOperation = false;
        selectedOperator = "";
        isResultShown = true;
    }

    // Sayının karesini alır.
    private void SquareButton_Clicked(object sender, EventArgs e)
    {
        if (!double.TryParse(DisplayLabel.Text, out double number))
            return;

        double result = number * number;

        HistoryLabel.Text = $"sqr({DisplayLabel.Text})";
        DisplayLabel.Text = result.ToString();

        isNewNumber = true;
    }

    // Sayının karekökünü hesaplar.
    private void SquareRootButton_Clicked(object sender, EventArgs e)
    {
        if (!double.TryParse(DisplayLabel.Text, out double number))
            return;

        // Negatif sayıların karekökünü almaz.
        if (number < 0)
        {
            DisplayLabel.Text = "Geçersiz giriş";
            HistoryLabel.Text = "";
            isNewNumber = true;
            return;
        }

        double result = Math.Sqrt(number);

        HistoryLabel.Text = $"√({DisplayLabel.Text})";
        DisplayLabel.Text = result.ToString();

        isNewNumber = true;
    }

    // Sayının 1'e bölümünü hesaplar.
    private void ReciprocalButton_Clicked(object sender, EventArgs e)
    {
        if (!double.TryParse(DisplayLabel.Text, out double number))
            return;

        if (number == 0)
        {
            DisplayLabel.Text = "Sıfıra bölünemez";
            HistoryLabel.Text = "";
            isNewNumber = true;
            return;
        }

        double result = 1 / number;

        HistoryLabel.Text = $"1/({DisplayLabel.Text})";
        DisplayLabel.Text = result.ToString();

        isNewNumber = true;
    }

    // Sayıyı pozitif veya negatif yapar.
    private void SignButton_Clicked(object sender, EventArgs e)
    {
        if (!double.TryParse(DisplayLabel.Text, out double number))
            return;

        if (number == 0)
            return;

        number = -number;

        DisplayLabel.Text = number.ToString();
        isNewNumber = false;
    }

    // Yüzde hesabını yapar.
    private void PercentButton_Clicked(object sender, EventArgs e)
    {
        if (!double.TryParse(DisplayLabel.Text, out double number))
            return;

        double result;

        // Toplama ve çıkarmada yüzdeyi ilk sayıya göre hesaplar.
        if (hasPendingOperation &&
            (selectedOperator == "+" || selectedOperator == "−"))
        {
            result = firstNumber * number / 100;
        }
        else
        {
            result = number / 100;
        }

        DisplayLabel.Text = result.ToString();
        isNewNumber = false;
    }
}