﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week7Sample.Model
{
    public class ResponseObject<T>
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }

        public List<string> ErrorMessages { get; set; }

        public T Data { get; set; }

        public ResponseObject()
        {
            ErrorMessages = new List<string>();
        }
    }
}
