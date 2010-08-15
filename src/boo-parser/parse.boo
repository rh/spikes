# Run with booi parse.boo -r:..\..\lib\ICSharpCode.NRefactory.dll

import ICSharpCode.NRefactory
import ICSharpCode.NRefactory.Ast
import ICSharpCode.NRefactory.Visitors

class Visitor(AbstractAstVisitor):

  override def VisitMethodDeclaration(method as MethodDeclaration, data):
    print "Method:   ${method.Name}"
    return method

  override def VisitPropertyDeclaration(property as PropertyDeclaration, data):
    print "Property: ${property.Name}"
    return property

parser = ParserFactory.CreateParser("Program.cs")
parser.Parse()

unit = parser.CompilationUnit
unit.AcceptVisitor(Visitor(), null)