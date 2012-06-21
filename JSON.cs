using System;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Text;

namespace IMV
{
	public class JSON
	{
        private const byte TOKEN_NONE = 0; // Парсинг завершён
		private const byte BRACE_OPEN = 1; // Фигурная скобка открывается (далее следует объект)
        private const byte BRACE_CLOSE = 2; // Фигурная скобка закрывается
        private const byte BRACKET_OPEN = 3; // Квадратная скобка открывается (далее следует массив)
        private const byte BRACKET_CLOSE = 4; // Квадратная скобка закрывается
        private const byte COLON = 5; // Двоеточие (для пар имя-значение в объекте)
        private const byte COMMA = 6; // Кавычки (для строки)
        private const byte STRING = 7; // Строка
        private const byte NUMBER = 8; // Число

		/// <summary>
		/// Парсит JSON строку
		/// </summary>
		/// <param name="json">JSON строка</param>
		/// <param name="success">Успешно ли выполнение</param>
		/// <returns>Объект. Либо Hashtable, либо ArrayList, либо строка, либо число</returns>
		public static object JsonDecode(string json)
		{
			bool success = true;
			if (json != null) // Если не пустая
            {
                char[] charArray = json.ToCharArray(); // Преобразуем строку в массив символов
                int index = 0; // Выставляем "курсор" на нулевую позицию
				object value = ParseValue(charArray, ref index, ref success); // Парсим строку
				return value;
            } 
            else 
            {
                return null;
            }
		}

        /// <summary>
        /// Метод определяет, какие данные записаны (массив, строка, число, или объект) и вызывает соответствующую функцию
        /// </summary>
        /// <param name="json">Массив символов, представляющий JSON строку</param>
        /// <param name="index">Положение "курсора" в строке</param>
        /// <param name="success">Успешно ли выполнение</param>
        /// <returns>Объект. Либо Hashtable, либо ArrayList, либо строка, либо число</returns>
        protected static object ParseValue(char[] json, ref int index, ref bool success)
        {
            switch (LookAhead(json, index)) // Смотрим какой символ следующий
            {
                case JSON.STRING: // Если кавычки
                    return ParseString(json, ref index, ref success);
                case JSON.NUMBER: // Если число
                    return ParseNumber(json, ref index);
                case JSON.BRACE_OPEN: // Если фигурная скобка
                    return ParseObject(json, ref index, ref success);
                case JSON.BRACKET_OPEN: // Если квадратная скобка
                    return ParseArray(json, ref index, ref success);
                case JSON.TOKEN_NONE: // Ничего нет, парсинг завершён
                    break;
            }

            success = false;
            return null;
        }

        /// <summary>
        /// Метод парсит объект (пары имя-значение)
        /// </summary>
        /// <param name="json">Массив символов, представляющий JSON строку</param>
        /// <param name="index">Положение "курсора" в строке</param>
        /// <param name="success">Успешно ли выполнение</param>
        /// <returns>Hashtable, в которой имя - ключ</returns>

		protected static Hashtable ParseObject(char[] json, ref int index, ref bool success)
		{
			Hashtable table = new Hashtable();
			byte token;

			NextToken(json, ref index); // сдвигаемся на позицию вправо, так как сейчас на открытой фигурной скобке

			bool done = false;
			while (!done) 
            {
				token = LookAhead(json, index); // просматриваем следующий символ
				if (token == JSON.TOKEN_NONE) // если строка закончилась
                {
					success = false;
					return null;
				} 
                else if (token == JSON.COMMA) // если кавычки
                {
					NextToken(json, ref index); // сдвигаемся на позицию вправо
				} 
                else if (token == JSON.BRACE_CLOSE) // если фигурная скобка закрывается
                {
					NextToken(json, ref index); // сдвигаемся на позицию вправо
					return table; // возвращаем таблицу пар имя-значение
				}
                else 
                {
					string name = ParseString(json, ref index, ref success); // парсим имя
					if (!success) 
                    {
						success = false;
						return null;
					}

					token = NextToken(json, ref index); // сдвигаемся на позицию вправо
					if (token != JSON.COLON) // если не двоеточие, значит ошибка в строке
                    {
						success = false;
						return null;
					}

					object value = ParseValue(json, ref index, ref success); // парсим значение, сопоставленное имени
					if (!success) 
                    {
						success = false;
						return null;
					}

					table[name] = value; // добавляем новую запись
				}
			}

			return table;
		}

        /// <summary>
        /// Метод парсит массив (множество значений, идущих подряд)
        /// </summary>
        /// <param name="json">Массив символов, представляющий JSON строку</param>
        /// <param name="index">Положение "курсора" в строке</param>
        /// <param name="success">Успешно ли выполнение</param>
        /// <returns>ArrayList, массив в котором расположены значения в порядке их следования в строке</returns>

		protected static ArrayList ParseArray(char[] json, ref int index, ref bool success)
		{
			ArrayList array = new ArrayList();

			NextToken(json, ref index); // сдвигаемся на позицию вправо, так как сейчас на позиции квадратной скобки

			bool done = false;
			while (!done)
            {
                byte token = LookAhead(json, index); // просматриваем следующий символ
                if (token == JSON.TOKEN_NONE) // если строка закончилась
                {
					success = false;
					return null;
				}
                else if (token == JSON.COMMA) // если кавычки
                {
                    NextToken(json, ref index); // сдвигаемся на позицию вправо
				}
                else if (token == JSON.BRACKET_CLOSE) // если квадратная скобка закрывается
                {
                    NextToken(json, ref index); // сдвигаемся на позицию вправо
                    break; // Выходим из цикла
				} 
                else 
                {
					object value = ParseValue(json, ref index, ref success); // парсим значение (рекурсия)
					if (!success) 
                    {
						return null;
					}

					array.Add(value); // добавляем значение в массив
				}
			}

			return array;
		}

        /// <summary>
        /// Метод парсит строку
        /// </summary>
        /// <param name="json">Массив символов, представляющий JSON строку</param>
        /// <param name="index">Положение "курсора" в строке</param>
        /// <param name="success">Успешно ли выполнение</param>
        /// <returns>Строка, которая была извлечена</returns>

		protected static string ParseString(char[] json, ref int index, ref bool success)
		{
			StringBuilder s = new StringBuilder(2000);
			char c;

			EatWhitespace(json, ref index); // игнорируем символны табуляции, пробелы и прочие
			
			c = json[index++]; // двигаемся вправо, так как стоим на кавычке

			bool complete = false;
			while (!complete) 
            {

				if (index == json.Length) // если конец массива
                {
					break;
				}

				c = json[index++]; // сдвигаемся вправо
				if (c == '"') // если кавычка, значит строка закончилась
                {
					complete = true;
					break;
				} 
                else if (c == '\\') // если обратный слэш, значит какой-то спецсимвол
                {

					if (index == json.Length) // если конец строки, выходим
                    {
						break;
					}
					c = json[index++]; // передвигаемся вправо
					if (c == '"') // если кавычка...
                    {
						s.Append('"');
					} 
                    else if (c == '\\') 
                    {
						s.Append('\\');
					} 
                    else if (c == '/') 
                    {
						s.Append('/');
					} 
                    else if (c == 'b') 
                    {
						s.Append('\b');
					} 
                    else if (c == 'f') 
                    {
						s.Append('\f');
					} 
                    else if (c == 'n') 
                    {
						s.Append('\n');
					} 
                    else if (c == 'r') 
                    {
						s.Append('\r');
					} 
                    else if (c == 't') 
                    {
						s.Append('\t');
					}
				} 
                else 
                {
					s.Append(c); // пристыковываем обычный символ
				}
			}

			if (!complete) 
            {
				success = false;
				return null;
			}

			return s.ToString();
		}

        /// <summary>
        /// Метод парсит число
        /// </summary>
        /// <param name="json">Массив символов, представляющий JSON строку</param>
        /// <param name="index">Положение "курсора" в строке</param>
        /// <returns>Числов, которое было извлечено</returns>

		protected static double ParseNumber(char[] json, ref int index)
		{
			EatWhitespace(json, ref index); // игнорируем символы табуляции, пробелы и прочие

			int lastIndex = GetLastIndexOfNumber(json, index); // ищем индекс последней цифры числа и возвращаем
			int charLength = (lastIndex - index) + 1;
			char[] numberCharArray = new char[charLength]; // массив для символов числа

			Array.Copy(json, index, numberCharArray, 0, charLength); // копируем символы с положения "курсора" до индекса последней цифры числа
			index = lastIndex + 1;
			return Double.Parse(new string(numberCharArray), CultureInfo.InvariantCulture); // пытаемся перевести в тип double
		}

        /// <summary>
        /// Метод ищет позицию, на которой оканчивается число
        /// </summary>
        /// <param name="json">Массив символов, представляющий JSON строку</param>
        /// <param name="index">Положение "курсора" в строке</param>
        /// <returns></returns>

		protected static int GetLastIndexOfNumber(char[] json, int index)
		{
			int lastIndex;

			for (lastIndex = index; lastIndex < json.Length; lastIndex++) // идём с положения "курсора" до конца строки
            {
                if ("0123456789+-.eE".IndexOf(json[lastIndex]) == -1) // если строка "0123456789+-.eE" не содержит символа, находящегося на позиции lastIndex, то числов закончилось
                {
					break;
				}
			}
			return lastIndex - 1;
		}

        /// <summary>
        /// Метод игнорирует знаки табуляции, пробелы, возвращения каретки
        /// </summary>
        /// <param name="json">Массив символов, представляющий JSON строку</param>
        /// <param name="index">Положение "курсора" в строке</param>

		protected static void EatWhitespace(char[] json, ref int index)
		{
			for (; index < json.Length; index++) // передвигаем "курсор" пока...
            {
                if (" \t\n\r".IndexOf(json[index]) == -1) // символ с индексом index содержится в строке " \t\n\r" 
                {
					break;
				}
			}
		}

        /// <summary>
        /// Метод запрашивает следующий знак в строке
        /// </summary>
        /// <param name="json">Массив символов, представляющий JSON строку</param>
        /// <param name="index">Положение "курсора" в строке</param>
        /// <returns>Возвращет описание следующего знака (кавычка, фигурная скобка, квадратная скобка)</returns>

		protected static byte LookAhead(char[] json, int index)
		{
			int saveIndex = index;
			return NextToken(json, ref saveIndex);
		}

        /// <summary>
        /// Метод указывающий какой символ идёт следующим в строке
        /// </summary>
        /// <param name="json">Массив символов, представляющий JSON строку</param>
        /// <param name="index">Положение "курсора" в строке</param>
        /// <returns>Возвращает константное значение, показывающее какой символ будет в строке следующим</returns>

		protected static byte NextToken(char[] json, ref int index)
		{
			EatWhitespace(json, ref index); // Пропуск ненужных пробелов, символов табуляции и возврата каретки

			if (index == json.Length) // если индекс равен длине строки, значит строка закончилась, конец парсинга 
            {
				return JSON.TOKEN_NONE;
			}
			
			char c = json[index];
			index++;
			switch (c) 
            {
				case '{': // если фигурная скобка открывается, возвращаем константу с соответствующим значением
					return JSON.BRACE_OPEN;
                case '}': // если фигурная скобка закрывается, возвращаем константу с соответствующим значением
					return JSON.BRACE_CLOSE;
                case '[': // если квадратная скобка открывается, возвращаем константу с соответствующим значением
					return JSON.BRACKET_OPEN;
                case ']': // если квадратная скобка закрывается, возвращаем константу с соответствующим значением
					return JSON.BRACKET_CLOSE;
                case ',': // если на позиции запятая, возвращаем константу с соответствующим значением
					return JSON.COMMA;
                case '"': // если на позиции кавычки, возвращаем константу с соответствующим значением
					return JSON.STRING;
				case '0': case '1': case '2': case '3': case '4': 
				case '5': case '6': case '7': case '8': case '9':
                case '-': // если на поцизии цифра, возвращаем константу с соответствующим значением
					return JSON.NUMBER;
                case ':': // если на позиции двоеточие, возвращаем константу с соответствующим значением
					return JSON.COLON;
			}
			index--;

			return JSON.TOKEN_NONE;
		}
	}
}
