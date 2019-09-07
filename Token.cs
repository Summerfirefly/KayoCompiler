﻿namespace KayoCompiler
{
    class Token
    {
        public Tag Tag { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"<{Tag}, {Value}>";
        }
    }
}
