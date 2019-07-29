﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeQuality.Analyzers.QualityGuidelines;
using Test.Utilities;
using Xunit;

namespace Microsoft.CodeQuality.Analyzers.UnitTests.QualityGuidelines
{
    public partial class ReferingToObjectAndReassigningItInTheSameStatementTests : DiagnosticAnalyzerTestBase
    {
        protected override DiagnosticAnalyzer GetBasicDiagnosticAnalyzer()
        {
            return new ReferingToObjectAndReassigningItInTheSameStatement();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ReferingToObjectAndReassigningItInTheSameStatement();
        }

        private DiagnosticResult GetCSharpResultAt(int line, int column, string symbolName)
        {
            return GetCSharpResultAt(line, column, ReferingToObjectAndReassigningItInTheSameStatement.Rule, symbolName);
        }

        private DiagnosticResult GetBasicResultAt(int line, int column, string symbolName)
        {
            return GetBasicResultAt(line, column, ReferingToObjectAndReassigningItInTheSameStatement.Rule, symbolName);
        }

        [Fact]
        public void CSharpReassignLocalVariableAndReferToItsField()
        {
            VerifyCSharp(@"
public class C
{
    public C Field;
}

public class Test
{
    public void Method()
    {
        C a, b;
        a.Field = a = b;
    }
}
",
            GetCSharpResultAt(12, 9, "a"));
        }

        [Fact]
        public void CSharpReassignLocalVariableAndReferToItsProperty()
        {
            VerifyCSharp(@"
public class C
{
    public C Property { get; set; }
}

public class Test
{
    public void Method()
    {
        C a, b, c;
        a.Property = c = a = b;
    }
}
",
            GetCSharpResultAt(12, 9, "a"));
        }

        [Fact]
        public void CSharpReassignLocalVariablesPropertyAndReferToItsProperty()
        {
            VerifyCSharp(@"
public class C
{
    public C Property { get; set; }
}

public class Test
{
    public void Method()
    {
        C a, b;
        a.Property.Property = a.Property = b;
    }
}
",
            GetCSharpResultAt(12, 9, "Property"));
        }

        [Fact]
        public void CSharpReassignLocalVariableAndItsPropertyAndReferToItsProperty()
        {
            VerifyCSharp(@"
public class C
{
    public C Property { get; set; }
}

public class Test
{
    public void Method()
    {
        C a, b;
        a.Property.Property = a.Property = a = b;
    }
}
",
            GetCSharpResultAt(12, 9, "Property"),
            GetCSharpResultAt(12, 31, "a"));
        }

        [Fact]
        public void CSharpReferToFieldOfReferenceTypeLocalVariableAfterItsReassignment()
        {
            VerifyCSharp(@"
public class C
{
    public C Field;
}


public class Test
{
    static C x, y;

    public void Method()
    {
        x.Field = x = y;
    }
}
",
            GetCSharpResultAt(14, 9, "x"));
        }

        [Fact]
        public void CSharpReassignGlobalVariableAndReferToItsField()
        {
            VerifyCSharp(@"
public class C
{
    public C Property { get; set; }
}


public class Test
{
    static C x, y;

    public void Method()
    {
        x.Property.Property = x.Property = y;
    }
}
",
            GetCSharpResultAt(14, 9, "Property"));
        }

        [Fact]
        public void CSharpReassignGlobalVariableAndItsPropertyAndReferToItsProperty()
        {
            VerifyCSharp(@"
public class C
{
    public C Property { get; set; }
}


public class Test
{
    static C x, y;

    public void Method()
    {
        x.Property.Property = x.Property = x = y;
    }
}
",
            GetCSharpResultAt(14, 9, "Property"),
            GetCSharpResultAt(14, 31, "x"));
        }


        [Fact]
        public void CSharpReassignGlobalPropertyAndItsPropertyAndReferToItsProperty()
        {
            VerifyCSharp(@"
public class C
{
    public C Property { get; set; }
}


public class Test
{
    static C x { get; set; } 
    static C y { get; set; }

    public void Method()
    {
        x.Property.Property = x.Property = x = y;
    }
}
",
            GetCSharpResultAt(15, 9, "Property"),
            GetCSharpResultAt(15, 31, "x"));
        }

        [Fact]
        public void CSharpReassignSecondLocalVariableAndReferToItsPropertyOfFirstVariable()
        {
            VerifyCSharp(@"
public class C
{
    public C Property { get; set; }
}


public class Test
{
    public void Method()
    {
        C a, b;
        a.Property = b = a;
    }
}
");
        }

        [Fact]
        public void CSharpReassignLocalValueTypeVariableAndReferToItsField()
        {
            VerifyCSharp(@"
public struct S
{
    public S Property { get; set; }
}


public class Test
{
    public void Method()
    {
        S a, b;
        a.Field = a = b;
    }
}
");
        }

        [Fact]
        public void CSharpReassignLocalValueTypeVariableAndReferToItsProperty()
        {
            VerifyCSharp(@"
public struct S
{
    public S Field;
}


public class Test
{
    public void Method()
    {
        S a, b;
        a.Property = c = a = b;
    }
}
");
        }
    }
}
