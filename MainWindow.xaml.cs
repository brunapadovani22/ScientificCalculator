using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CalculadoraCientifica.Core;

namespace CalculadoraCientifica
{
    public partial class MainWindow : Window
    {
        private AngleMode currentAngleMode = AngleMode.Degrees;

        public MainWindow()
        {
            InitializeComponent();

            // Força a janela a ter foco ao abrir para o teclado funcionar de primeira
            Loaded += (s, e) => Focus();
        }

        private void AppendText(string text)
        {
            if (Display.Text == "0" && text != ",")
            {
                Display.Text = text;
            }
            else
            {
                Display.Text += text;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string content = button.Content?.ToString() ?? "";

                switch (content)
                {
                    case "C":
                        Display.Text = "";
                        break;
                    case "⌫":
                        if (Display.Text.Length > 0)
                            Display.Text = Display.Text.Substring(0, Display.Text.Length - 1);
                        break;
                    case "=":
                        Calculate();
                        break;
                    case "sin":
                    case "cos":
                    case "tan":
                    case "ln":
                        AppendFunction(content);
                        break;
                    case "x^y":
                        Display.Text += "^";
                        break;
                    case "DEG":
                        currentAngleMode = AngleMode.Radians;
                        button.Content = "RAD";
                        break;
                    case "RAD":
                        currentAngleMode = AngleMode.Degrees;
                        button.Content = "DEG";
                        break;
                    default:
                        AppendText(content);
                        break;
                }
            }
        }

        private void AppendFunction(string func)
        {
            if (!string.IsNullOrEmpty(Display.Text))
            {
                char lastChar = Display.Text.Last();
                if (char.IsDigit(lastChar) || lastChar == ')' || lastChar == ',')
                {
                    Display.Text += "×";
                }
            }
            Display.Text += func + "(";
        }

        private void Calculate()
        {
            if (string.IsNullOrWhiteSpace(Display.Text)) return;

            try
            {
                string expression = Display.Text.Replace("×", "*").Replace("÷", "/");
                double result = Parser.Evaluate(expression, currentAngleMode);

                Display.Text = result.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro de Cálculo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // PreviewKeyDown captura os atalhos ANTES de qualquer outro elemento da tela roubar o foco
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            // Números da fileira superior (D0-D9)
            if (e.Key >= Key.D0 && e.Key <= Key.D9 && Keyboard.Modifiers != ModifierKeys.Shift)
            {
                AppendText((e.Key - Key.D0).ToString());
                e.Handled = true;
            }
            // Números do Teclado Numérico (NumPad)
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                AppendText((e.Key - Key.NumPad0).ToString());
                e.Handled = true;
            }
            // Operadores do NumPad e teclado principal
            else if (e.Key == Key.Add || (e.Key == Key.OemPlus && Keyboard.Modifiers != ModifierKeys.Shift))
            {
                AppendText("+");
                e.Handled = true;
            }
            else if (e.Key == Key.Subtract || e.Key == Key.OemMinus)
            {
                AppendText("-");
                e.Handled = true;
            }
            else if (e.Key == Key.Multiply)
            {
                AppendText("×");
                e.Handled = true;
            }
            else if (e.Key == Key.Divide)
            {
                AppendText("÷");
                e.Handled = true;
            }
            else if (e.Key == Key.Decimal || e.Key == Key.OemComma || e.Key == Key.OemPeriod)
            {
                AppendText(",");
                e.Handled = true;
            }
            // Parênteses: Shift + 8 '(' e Shift + 9 ')'
            else if (e.Key == Key.D8 && Keyboard.Modifiers == ModifierKeys.Shift)
            {
                AppendText("(");
                e.Handled = true;
            }
            else if (e.Key == Key.D9 && Keyboard.Modifiers == ModifierKeys.Shift)
            {
                AppendText(")");
                e.Handled = true;
            }
            // Enter e Numpad Enter para calcular
            else if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                Calculate();
                e.Handled = true;
            }
            // Backspace apaga caractere
            else if (e.Key == Key.Back)
            {
                if (Display.Text.Length > 0)
                    Display.Text = Display.Text.Substring(0, Display.Text.Length - 1);
                e.Handled = true;
            }
            // Esc limpa o display
            else if (e.Key == Key.Escape)
            {
                Display.Text = "";
                e.Handled = true;
            }
        }
    }
}