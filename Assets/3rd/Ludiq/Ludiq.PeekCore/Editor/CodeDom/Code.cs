using System;
using System.Collections.Generic;
using System.Linq;

namespace Ludiq.PeekCore.CodeDom
{
	public static class Code
	{
		public static CodePrimitiveExpression Primitive(object o) => new CodePrimitiveExpression(o);
		public static CodeVariableReferenceExpression VarRef(string name) => new CodeVariableReferenceExpression(name);
		public static CodeThisReferenceExpression ThisRef => new CodeThisReferenceExpression();
		public static CodeBaseReferenceExpression BaseRef => new CodeBaseReferenceExpression();

		public static CodeArgumentDirectionExpression ArgumentDirection(CodeParameterDirection direction, CodeExpression target) => new CodeArgumentDirectionExpression(direction, target);
		public static CodeArgumentDirectionExpression RefArgument(CodeExpression target) => ArgumentDirection(CodeParameterDirection.Ref, target);
		public static CodeArgumentDirectionExpression OutArgument(CodeExpression target) => ArgumentDirection(CodeParameterDirection.Out, target);

		public static CodeFieldReferenceExpression Field(this CodeExpression target, string name) => new CodeFieldReferenceExpression(target, name);
		public static CodeMethodReferenceExpression Method(this CodeExpression target, string name, params CodeTypeReference[] typeArguments) => new CodeMethodReferenceExpression(target, name, typeArguments);
		public static CodeMethodReferenceExpression Method(this CodeExpression target, string name, IEnumerable<CodeTypeReference> typeArguments) => new CodeMethodReferenceExpression(target, name, typeArguments);

		public static CodeMethodInvokeExpression Invoke(this CodeMethodReferenceExpression method, params CodeExpression[] arguments) => new CodeMethodInvokeExpression(method, arguments);
		public static CodeMethodInvokeExpression Invoke(this CodeMethodReferenceExpression method, IEnumerable<CodeExpression> arguments) => new CodeMethodInvokeExpression(method, arguments);

		public static CodeNamedArgumentExpression NamedArgument(string name, CodeExpression value) => new CodeNamedArgumentExpression(name, value);

		public static CodeConditionalExpression Conditional(CodeExpression condition, CodeExpression trueExpression, CodeExpression falseExpression) => new CodeConditionalExpression(condition, trueExpression, falseExpression);

		public static CodeBinaryOperatorExpression BinaryOp(this CodeExpression left, CodeBinaryOperatorType op, CodeExpression right) => new CodeBinaryOperatorExpression(left, op, right);
		public static CodeBinaryOperatorExpression Add(this CodeExpression left, CodeExpression right) => BinaryOp(left, CodeBinaryOperatorType.Add, right);
		public static CodeBinaryOperatorExpression Subtract(this CodeExpression left, CodeExpression right) => BinaryOp(left, CodeBinaryOperatorType.Subtract, right);
		public static CodeBinaryOperatorExpression Multiply(this CodeExpression left, CodeExpression right) => BinaryOp(left, CodeBinaryOperatorType.Multiply, right);
		public static CodeBinaryOperatorExpression Divide(this CodeExpression left, CodeExpression right) => BinaryOp(left, CodeBinaryOperatorType.Divide, right);
		public static CodeBinaryOperatorExpression Modulo(this CodeExpression left, CodeExpression right) => BinaryOp(left, CodeBinaryOperatorType.Modulo, right);
		public static CodeBinaryOperatorExpression Equal(this CodeExpression left, CodeExpression right) => BinaryOp(left, CodeBinaryOperatorType.Equality, right);
		public static CodeBinaryOperatorExpression NotEqual(this CodeExpression left, CodeExpression right) => BinaryOp(left, CodeBinaryOperatorType.Inequality, right);
		public static CodeBinaryOperatorExpression BitwiseOr(this CodeExpression left, CodeExpression right) => BinaryOp(left, CodeBinaryOperatorType.BitwiseOr, right);
		public static CodeBinaryOperatorExpression BitwiseAnd(this CodeExpression left, CodeExpression right) => BinaryOp(left, CodeBinaryOperatorType.BitwiseAnd, right);
		public static CodeBinaryOperatorExpression BitwiseXor(this CodeExpression left, CodeExpression right) => BinaryOp(left, CodeBinaryOperatorType.BitwiseXor, right);
		public static CodeBinaryOperatorExpression LogicalOr(this CodeExpression left, CodeExpression right) => BinaryOp(left, CodeBinaryOperatorType.LogicalOr, right);
		public static CodeBinaryOperatorExpression LogicalAnd(this CodeExpression left, CodeExpression right) => BinaryOp(left, CodeBinaryOperatorType.LogicalAnd, right);
		public static CodeBinaryOperatorExpression LessThan(this CodeExpression left, CodeExpression right) => BinaryOp(left, CodeBinaryOperatorType.LessThan, right);
		public static CodeBinaryOperatorExpression LessThanOrEqual(this CodeExpression left, CodeExpression right) => BinaryOp(left, CodeBinaryOperatorType.LessThanOrEqual, right);
		public static CodeBinaryOperatorExpression GreaterThan(this CodeExpression left, CodeExpression right) => BinaryOp(left, CodeBinaryOperatorType.GreaterThan, right);
		public static CodeBinaryOperatorExpression GreaterThanOrEqual(this CodeExpression left, CodeExpression right) => BinaryOp(left, CodeBinaryOperatorType.GreaterThanOrEqual, right);
		public static CodeBinaryOperatorExpression BitwiseShiftLeft(this CodeExpression left, CodeExpression right) => BinaryOp(left, CodeBinaryOperatorType.BitwiseShiftLeft, right);
		public static CodeBinaryOperatorExpression BitwiseShiftRight(this CodeExpression left, CodeExpression right) => BinaryOp(left, CodeBinaryOperatorType.BitwiseShiftRight, right);
		public static CodeBinaryOperatorExpression Is(this CodeExpression left, CodeExpression right) => BinaryOp(left, CodeBinaryOperatorType.Is, right);
		public static CodeBinaryOperatorExpression As(this CodeExpression left, CodeExpression right) => BinaryOp(left, CodeBinaryOperatorType.As, right);
		public static CodeBinaryOperatorExpression NullCoalesce(this CodeExpression left, CodeExpression right) => BinaryOp(left, CodeBinaryOperatorType.NullCoalesce, right);

		public static CodeIndexExpression Index(this CodeExpression operand, params CodeExpression[] indices) => new CodeIndexExpression(operand, indices);
		public static CodeIndexExpression Index(this CodeExpression operand, IEnumerable<CodeExpression> indices) => new CodeIndexExpression(operand, indices);

		public static CodeAssignmentExpression Assign(this CodeExpression left, CodeExpression right) => new CodeAssignmentExpression(left, right);

		public static CodeCompoundAssignmentExpression CompoundAssign(this CodeExpression left, CodeCompoundAssignmentOperatorType op, CodeExpression right) => new CodeCompoundAssignmentExpression(left, op, right);
		public static CodeCompoundAssignmentExpression AddAssign(this CodeExpression left, CodeExpression right) => CompoundAssign(left, CodeCompoundAssignmentOperatorType.Add, right);
		public static CodeCompoundAssignmentExpression SubtractAssign(this CodeExpression left, CodeExpression right) => CompoundAssign(left, CodeCompoundAssignmentOperatorType.Subtract, right);
		public static CodeCompoundAssignmentExpression MultiplyAssign(this CodeExpression left, CodeExpression right) => CompoundAssign(left, CodeCompoundAssignmentOperatorType.Multiply, right);
		public static CodeCompoundAssignmentExpression DivideAssign(this CodeExpression left, CodeExpression right) => CompoundAssign(left, CodeCompoundAssignmentOperatorType.Divide, right);
		public static CodeCompoundAssignmentExpression ModuloAssign(this CodeExpression left, CodeExpression right) => CompoundAssign(left, CodeCompoundAssignmentOperatorType.Modulo, right);
		public static CodeCompoundAssignmentExpression BitwiseOrAssign(this CodeExpression left, CodeExpression right) => CompoundAssign(left, CodeCompoundAssignmentOperatorType.BitwiseOr, right);
		public static CodeCompoundAssignmentExpression BitwiseAndAssign(this CodeExpression left, CodeExpression right) => CompoundAssign(left, CodeCompoundAssignmentOperatorType.BitwiseAnd, right);
		public static CodeCompoundAssignmentExpression BitwiseXorAssign(this CodeExpression left, CodeExpression right) => CompoundAssign(left, CodeCompoundAssignmentOperatorType.BitwiseXor, right);
		public static CodeCompoundAssignmentExpression BitwiseShiftLeftAssign(this CodeExpression left, CodeExpression right) => CompoundAssign(left, CodeCompoundAssignmentOperatorType.BitwiseShiftLeft, right);
		public static CodeCompoundAssignmentExpression BitwiseShiftRightAssign(this CodeExpression left, CodeExpression right) => CompoundAssign(left, CodeCompoundAssignmentOperatorType.BitwiseShiftRight, right);
		
		public static CodeUnaryOperatorExpression UnaryOp(this CodeExpression operand, CodeUnaryOperatorType op) => new CodeUnaryOperatorExpression(op, operand);
		public static CodeUnaryOperatorExpression Positive(this CodeExpression operand) => UnaryOp(operand, CodeUnaryOperatorType.Positive);
		public static CodeUnaryOperatorExpression Negative(this CodeExpression operand) => UnaryOp(operand, CodeUnaryOperatorType.Negative);
		public static CodeUnaryOperatorExpression LogicalNot(this CodeExpression operand) => UnaryOp(operand, CodeUnaryOperatorType.LogicalNot);
		public static CodeUnaryOperatorExpression BitwiseNot(this CodeExpression operand) => UnaryOp(operand, CodeUnaryOperatorType.BitwiseNot);
		public static CodeUnaryOperatorExpression PreIncrement(this CodeExpression operand) => UnaryOp(operand, CodeUnaryOperatorType.PreIncrement);
		public static CodeUnaryOperatorExpression PreDecrement(this CodeExpression operand) => UnaryOp(operand, CodeUnaryOperatorType.PreDecrement);
		public static CodeUnaryOperatorExpression AddressOf(this CodeExpression operand) => UnaryOp(operand, CodeUnaryOperatorType.AddressOf);
		public static CodeUnaryOperatorExpression Dereference(this CodeExpression operand) => UnaryOp(operand, CodeUnaryOperatorType.Dereference);
		public static CodeUnaryOperatorExpression PostIncrement(this CodeExpression operand) => UnaryOp(operand, CodeUnaryOperatorType.PostIncrement);
		public static CodeUnaryOperatorExpression PostDecrement(this CodeExpression operand) => UnaryOp(operand, CodeUnaryOperatorType.PostDecrement);

		public static CodeCastExpression Cast(this CodeExpression operand, CodeTypeReference type) => new CodeCastExpression(type, operand);

		public static CodeLambdaExpression Lambda(IEnumerable<string> parameters, CodeExpression value) => new CodeLambdaExpression(parameters.Select(parameter => ParamDecl(VarType, parameter)), value);
		public static CodeLambdaExpression Lambda(IEnumerable<CodeParameterDeclaration> parameters, CodeExpression value) => new CodeLambdaExpression(parameters, value);
		public static CodeLambdaExpression Lambda(IEnumerable<string> parameters, IEnumerable<CodeStatement> statements) => new CodeLambdaExpression(parameters.Select(parameter => ParamDecl(VarType, parameter)), statements);
		public static CodeLambdaExpression Lambda(IEnumerable<CodeParameterDeclaration> parameters, IEnumerable<CodeStatement> statements) => new CodeLambdaExpression(parameters, statements);

		public static CodeObjectCreateExpression ObjectCreate(this CodeTypeReference type, params CodeExpression[] arguments) => new CodeObjectCreateExpression(type, arguments);
		public static CodeObjectCreateExpression ObjectCreate(this CodeTypeReference type, IEnumerable<CodeExpression> arguments) => new CodeObjectCreateExpression(type, arguments);
		public static CodeObjectCreateExpression ObjectCreate(this CodeTypeReference type, IEnumerable<CodeExpression> arguments, IEnumerable<IEnumerable<CodeExpression>> collectionInitializerItems) => new CodeObjectCreateExpression(type, arguments, collectionInitializerItems);
		public static CodeObjectInitializerExpression ObjectInitializer(this CodeTypeReference type, IEnumerable<KeyValuePair<string, CodeExpression>> members) => new CodeObjectInitializerExpression(type, members);	
		public static CodeDefaultValueExpression DefaultValue(this CodeTypeReference type) => new CodeDefaultValueExpression(type);
		public static CodeDefaultValueExpression DefaultValue() => new CodeDefaultValueExpression();
		public static CodeArrayCreateExpression ArrayInitializer(this CodeTypeReference type, params CodeExpression[] initializer) => ArraySizedInitializer(type, Enumerable.Empty<CodeExpression>(), initializer);
		public static CodeArrayCreateExpression ArrayInitializer(this CodeTypeReference type, IEnumerable<CodeExpression> initializer) => ArraySizedInitializer(type, Enumerable.Empty<CodeExpression>(), initializer);
		public static CodeArrayCreateExpression ArrayOfSize(this CodeTypeReference type, params CodeExpression[] lengths) => ArraySizedInitializer(type, lengths, Enumerable.Empty<CodeExpression>());
		public static CodeArrayCreateExpression ArrayOfSize(this CodeTypeReference type, IEnumerable<CodeExpression> lengths) => ArraySizedInitializer(type, lengths, Enumerable.Empty<CodeExpression>());
		public static CodeArrayCreateExpression ArraySizedInitializer(this CodeTypeReference type, IEnumerable<CodeExpression> lengths, IEnumerable<CodeExpression> initializer) => new CodeArrayCreateExpression(type, lengths, initializer);
		public static CodeBracedInitializerExpression BracedInitializer(IEnumerable<CodeExpression> arguments) => new CodeBracedInitializerExpression(arguments);
		public static CodeTypeOfExpression TypeOf(this CodeTypeReference type) => new CodeTypeOfExpression(type);
		public static CodeTypeReferenceExpression Expression(this CodeTypeReference type) => new CodeTypeReferenceExpression(type);

		public static CodeTypeReference DeclarationType(Type type, CodeExpression initExpression)
		{
			var useExplicitType = initExpression is CodePrimitiveExpression primitive && primitive.Value == null ||
			                      initExpression is CodeLambdaExpression;

			return useExplicitType ? TypeRef(type) : VarType;
		}

		public static CodeTypeReference VarType => new CodeTypeReference("var");
		public static CodeTypeReference SByteType => new CodeTypeReference(typeof(sbyte));
		public static CodeTypeReference Int16Type => new CodeTypeReference(typeof(short));
		public static CodeTypeReference Int32Type => new CodeTypeReference(typeof(int));
		public static CodeTypeReference Int64Type => new CodeTypeReference(typeof(long));
		public static CodeTypeReference StringType => new CodeTypeReference(typeof(string));
		public static CodeTypeReference ObjectType => new CodeTypeReference(typeof(object));
		public static CodeTypeReference BoolType => new CodeTypeReference(typeof(bool));
		public static CodeTypeReference VoidType => new CodeTypeReference(typeof(void));
		public static CodeTypeReference CharType => new CodeTypeReference(typeof(char));
		public static CodeTypeReference ByteType => new CodeTypeReference(typeof(byte));
		public static CodeTypeReference UInt16Type => new CodeTypeReference(typeof(ushort));
		public static CodeTypeReference UInt32Type => new CodeTypeReference(typeof(uint));
		public static CodeTypeReference UInt64Type => new CodeTypeReference(typeof(ulong));
		public static CodeTypeReference FloatType => new CodeTypeReference(typeof(float));
		public static CodeTypeReference DoubleType => new CodeTypeReference(typeof(double));
		public static CodeTypeReference DecimalType => new CodeTypeReference(typeof(decimal));
		public static CodeTypeReference ShortType => new CodeTypeReference(typeof(short));
		public static CodeTypeReference IntType => new CodeTypeReference(typeof(int));
		public static CodeTypeReference LongType => new CodeTypeReference(typeof(long));
		public static CodeTypeReference UShortType => new CodeTypeReference(typeof(ushort));
		public static CodeTypeReference UIntType => new CodeTypeReference(typeof(uint));
		public static CodeTypeReference ULongType => new CodeTypeReference(typeof(ulong));
		public static CodeTypeReference TypeRef(Type type, bool global = false) => new CodeTypeReference(type, global);
		public static CodeTypeReference TypeRef(string name, bool global = false) => new CodeTypeReference(name, global);
		public static CodeTypeReference ArrayTypeRef(CodeTypeReference elementType, int rank = 1, bool global = false) => new CodeTypeReference(elementType, rank, global);
		
		public static CodeParameterDeclaration ParamDecl(CodeParameterDirection direction, CodeTypeReference type, string name) => new CodeParameterDeclaration(direction, type, name);
		public static CodeParameterDeclaration ParamDecl(CodeTypeReference type, string name) => new CodeParameterDeclaration(type, name);
		public static CodeVariableDeclarationStatement VarDecl(CodeTypeReference type, string name, CodeExpression initializer = null) => new CodeVariableDeclarationStatement(type, name, initializer);
		public static CodeStatement Statement(this CodeExpression expression) => new CodeExpressionStatement(expression);

		public static CodeUsingImport Using(string name) => new CodeUsingImport(name);

		public static CodeReturnStatement Return(CodeExpression expression) => new CodeReturnStatement(expression);
		public static CodeTupleExpression Tuple(params CodeExpression[] items) => new CodeTupleExpression(items);
		public static CodeTupleExpression Tuple(IEnumerable<CodeExpression> items) => new CodeTupleExpression(items);
	}
}
