
var pattern = new System.Text.RegularExpressions.Regex(
    @"20[0-3]\d((0[1-9])|(1[0-2]))((0[1-9])|([1-2]\d)|(3[0-1]))_\d+");

if (args.Length == 0) { return; }

List<string> list = new();
foreach (var arg in args)
{
    if (arg.Contains(";"))
    {
        list.AddRange(arg.Split(';'));
    }
    else
    {
        list.Add(arg);
    }
}

foreach (var path in list.Where(x => File.Exists(x)))
{
    string parent = Path.GetDirectoryName(path);
    string name = Path.GetFileName(path);

    var match = pattern.Match(name);
    if (match.Success)
    {
        string namePreText = name.Substring(0, match.Index) + DateTime.Today.ToString("yyyyMMdd_");
        string nameSufText = name.Substring(match.Index + match.Length);
        int digit = match.Value.Substring(9).Length;        //  yyyyMMdd_ で9文字
        int next = int.Parse(match.Value.Substring(9)) + 1;

        var todayNums = Directory.GetFiles(parent).Select(x =>
        {
            string tempName = Path.GetFileName(x);
            if (tempName.StartsWith(namePreText) && tempName.EndsWith(nameSufText))
            {
                string tempNum = tempName.Substring(
                    namePreText.Length,
                    tempName.Length - namePreText.Length - nameSufText.Length);
                return int.TryParse(tempNum, out int nn) ? nn : -1;
            }
            return -1;
        }).Where(x => x >= 0);
        if (todayNums.Count() > 0 && todayNums.Max() >= next)
        {
            next = todayNums.Max() + 1;
        }

        string newFilePath = Path.Combine(
            parent,
            namePreText + next.ToString().PadLeft(digit, '0') + nameSufText);
        File.Copy(path, newFilePath);
    }
}
