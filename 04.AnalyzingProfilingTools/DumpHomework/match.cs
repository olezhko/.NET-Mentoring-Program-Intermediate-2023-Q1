public static void Regex()
{
	string input = "11+++23";
	string pattern = @"(\d+)\s*([+\-*/])+\s*(\d+)";
	Regex regex = new Regex(pattern);
	Match match = regex.Match(input);
	int operand1 = int.Parse(match.Groups[1].Value);
	char op = match.Groups[2].Value[0];
	int operand2 = int.Parse(match.Groups[3].Value);
}

double num3 = Convert.ToDouble(tb.Text.Substring(num + 1, tb.Text.Length - num - 1).Trim('+', '-', '*', '/'));