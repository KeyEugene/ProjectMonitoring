using System;
using System.Collections.Generic;
using System.Text;

namespace Teleform.SqlServer.Formatting
{
    [Serializable()]
    public static class MoneyInWords
    {
        static readonly string[] numberName = { "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять" };

        public static string GetMoneyInWords(object value)
        {
			if (value is long || value is int || value is decimal || value is double)
			{
				//Int64.TryParse(value.ToString(), out v);
				var v = Convert.ToInt64(value);
				if (v > 500000000000)
					throw new ArgumentOutOfRangeException("value", "Размер входящего значения не может превышать  500 000 000 000");
			}
			else
				return string.Empty;

            long valueInt;
			try
			{
				valueInt = Convert.ToInt64(value);
			}
			catch (ArgumentOutOfRangeException)
			{
				throw new ArgumentOutOfRangeException("value", "Значение не может быть отрицательным.");
			}
			catch (ArgumentException)
			{
				throw new ArgumentException(
					string.Format("Неверный тип данных для числа. Тип данных: {0}. Переданное значение: {1}.",
						value.GetType().FullName,
						value), "value");
			}
			catch (Exception)
			{
				throw new Exception("Произшла ошибка преобразования денежных единиц.");
			}

            if (valueInt < 0)
                throw new ArgumentOutOfRangeException("value", "Значение не может быть отрицательным.");

            int modulo;

            var result = new StringBuilder();

            for (int i = 0; valueInt > 0; i++, valueInt = valueInt / 1000)
            {
                modulo = (int)(valueInt % 1000);
                if (modulo != 0)
                    result.Insert(0, GetThirdDigit(modulo / 100) + ((modulo % 100) / 10 == 1 ? GetFirstDigit(modulo % 10 + 10, i) : GetSecondDigit((modulo % 100) / 10) + GetFirstDigit(modulo % 10, i)));
                else if (i == 0)
                    result.Append(GetDischarge(modulo, i));
            }

            if (string.IsNullOrEmpty(result.ToString()))
                result.Append("ноль " + GetDischarge(0, 0));

            return result.ToString();
        }

        private static string GetThirdDigit(int hundred)
        {
            var result = string.Empty;
            if (hundred == 1)
                result = "сто";
            if (hundred == 2)
                result = "двести";
            if (hundred >= 3 && hundred < 5)
                result = numberName[hundred - 1] + "ста";
            else if (hundred >= 5)
                result = numberName[hundred - 1] + "сот";
            return result + (!string.IsNullOrEmpty(result) ? " " : string.Empty);
        }

        private static string GetSecondDigit(int ten)
        {
            var result = string.Empty;
            if (ten == 3 || ten == 2)
                result = numberName[ten - 1] + "дцать";
            else if (ten == 4)
                result = "сорок";
            if (ten >= 5 && ten < 9)
                result = numberName[ten - 1] + "десят";
            else if (ten == 9)
                result = "девяносто";

            return result + (!string.IsNullOrEmpty(result) ? " " : string.Empty);
        }

        private static string GetFirstDigit(int one, int i)
        {
            var result = string.Empty;
            if (one > 9)
            {
                if (one - 10 == 0)
                    result = "десять";
                else
                    result = (one - 10 > 4 || one - 10 == 2 ? numberName[one - 11].Substring(0, numberName[one - 11].Length - 1) + (one - 10 == 2 ? "е" : string.Empty) : numberName[one - 11]) + "надцать";
            }
            else
            {
                if (i == 1 && (one == 1 || one == 2))
                    result = one == 1 ? numberName[one - 1].Substring(0, numberName[one - 1].Length - 2) + "на" : numberName[one - 1].Substring(0, numberName[one - 1].Length - 1) + "е";
                else
                    result = one != 0 ? numberName[one - 1] : string.Empty;
            }
            
            return result + (!string.IsNullOrEmpty(result) ? " " : string.Empty) + GetDischarge(one, i);
        }

        private static string GetDischarge(int modulo, int i)
        {
            string[] lexem = { "рубл", "тысяч", "миллион", "миллиард", "триллион", "квадриллион", "квинтиллион" };

            if (i != 0)
            {
                if (modulo >= 5 || modulo == 0)
                    return i == 1 ? lexem[1] + " " : lexem[i] + "ов ";
                else if (modulo == 1)
                    return i == 1 ? lexem[1] + "а " : lexem[i] + " ";
                else if (modulo > 1 && modulo < 5)
                    return i == 1 ? lexem[1] + "и " : lexem[i] + "а ";
            }
            else
            {
                if (modulo > 4 || modulo == 0)
                    return lexem[0] + "ей";
                else if (modulo > 1 && modulo <= 4)
                    return lexem[0] + "я";
                else if (modulo == 1)
                    return lexem[0] + "ь";
            }
            return string.Empty;
        }
    }
}
