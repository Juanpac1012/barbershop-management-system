using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiKit.Models;
public class LanguageSelectItem
{
    public string Code { get; set; }
    public string Flag { get; set; }
    public string Name { get; set; }
    public bool IsRTL { get; set; }
}
