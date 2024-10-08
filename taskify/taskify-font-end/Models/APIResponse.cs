﻿using System.Net;

namespace taskify_font_end.Models
{
    public class APIResponse
    {
        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccess { get; set; } = true;
        public List<string> ErrorMessages { get; set; }
        public object Result { get; set; }

        public static implicit operator List<object>(APIResponse v)
        {
            throw new NotImplementedException();
        }
    }
}
