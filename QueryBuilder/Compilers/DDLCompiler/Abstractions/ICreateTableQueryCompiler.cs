﻿using System.Collections.Generic;
using System.Text;
using SqlKata.Contract.CreateTable;

namespace SqlKata.Compilers.DDLCompiler.Abstractions
{
    internal interface ICreateTableQueryCompiler
    {
        string CompileCreateTable(Query query);
    }
}
