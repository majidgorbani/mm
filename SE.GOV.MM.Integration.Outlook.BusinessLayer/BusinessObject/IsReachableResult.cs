﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.Outlook.BusinessLayer.BusinessObject
{
    public class IsReachableResult
    {
        public IsReachableResult() { }

        public string Id { get; set; }
        public string Name { get; set; }
        public string ServiceAdress { get; set; }
        public string UIAdress { get; set; }
        public bool IsReachable { get; set; }
    }
}
