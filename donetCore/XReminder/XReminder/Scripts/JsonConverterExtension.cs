using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class DateTimeConverter : IsoDateTimeConverter
{
    static public readonly DateTimeConverter Instance = new DateTimeConverter();

    private DateTimeConverter()
    {
        DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    }
}