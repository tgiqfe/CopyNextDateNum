
var pattern = new System.Text.RegularExpressions.Regex(
    @"20[0-3]\d((0[1-9])|(1[0-2]))((0[1-9])|([1-2]\d)|(3[0-1]))_\d+");

if (args.Length == 0) return;
var arguments = args.Aggregate(new List<string>(), (list, arg) =>
{
    if (arg.Contains(";"))
    {
        list.AddRange(arg.Split(';'));
    }
    else
    {
        list.Add(arg);
    }
    return list;
});

var files = arguments.Where(x => File.Exists(x)).
    Select(x => new { Path = x, Name = Path.GetFileName(x), Parent = Path.GetDirectoryName(x) }).
    ToList();

files.ForEach(x =>
{
    var match = pattern.Match(x.Name);
    if (match.Success)
    {
        string dateText = DateTime.Today.ToString("yyyyMMdd_");
        string namePreText = x.Name.Substring(0, match.Index) + dateText;
        string nameSufText = x.Name.Substring(match.Index + match.Length);
        string now = match.Value.Substring(dateText.Length);
        int digit = now.Length;
        int next = int.Parse(now) + 1;

        var todayNums = Directory.GetFiles(x.Parent).
            Select(xx =>
            {
                string tempName = Path.GetFileName(xx);
                if (tempName.StartsWith(namePreText) && tempName.EndsWith(nameSufText))
                {
                    string tempNum = tempName.Substring(
                        namePreText.Length,
                        tempName.Length - namePreText.Length - nameSufText.Length);
                    return int.TryParse(tempNum, out int nn) ? nn : -1;
                }
                return -1;
            }).
            Where(xx => xx >= 0);
        if (todayNums.Count() > 0 && todayNums.Max() >= next)
        {
            next = todayNums.Max() + 1;
        }

        string newFilePath = Path.Combine(
            x.Parent,
            namePreText + next.ToString().PadLeft(digit, '0') + nameSufText);
        File.Copy(x.Path, newFilePath);
    }
});
