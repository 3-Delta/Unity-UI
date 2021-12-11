public static class CodeDef
{
    // 花括号结尾
    public static string segmentHead = @"{
";
    public static string segmentTail = @"}";

    public static string namespaceHead = @"namespace {0}";

    public static string classHead = @"public class {0}";

    public static string classStaticHead = @"public static class {0}";

    public static string classSealedHead = @"public sealed class {0}";

    public static string classAbstractHead = @"public abstract class {0}";

    // 实现一个借口或者一个类
    public static string classHeadWithInherit1 = @"public class {0} : {1}";

    public static string classHeadWithInherit2 = @"public class {0} : {1} {2}";

    public static string classHeadWithInherit3 = @"public class {0} : {1} {2} {3}";

    public static string classHeadWithInherit4 = @"public class {0} : {1} {2} {3} {4}";

    public static string structHead = @"public struct {0}";

    public static string classPublicMember = @"public {0} {1};";
    public static string classProtectedMember = @"protected {0} {1};";
    public static string classPrivatedMember = @"private {0} {1};";

    public static string classPublicMemberWithValue = @"public {0} {1} = {2};";
    public static string classProtectedMemberWithValue = @"protected {0} {1} = {2};";
    public static string classPrivatedMemberWithValue = @"private {0} {1} = {2};";

    public static string classNewPublicMember = @"public new {0} {1};";
    public static string classNewProtectedMember = @"protected new {0} {1};";
    public static string classNewPrivatedMember = @"private new {0} {1};";

    public static string classNewPublicMemberWithValue = @"public new {0} {1} = {2};";
    public static string classNewProtectedMemberWithValue = @"protected new {0} {1} = {2};";
    public static string classNewPrivatedMemberWithValue = @"private new {0} {1} = {2};";

    public static string classStaticPublicMember = @"public static {0} {1};";
    public static string classStaticProtectedMember = @"protected Static {0} {1};";
    public static string classStaticPrivatedMember = @"private Static {0} {1};";

    public static string classStaticPublicMemberWithValue = @"public static {0} {1} = {2};";
    public static string classStaticProtectedMemberWithValue = @"protected Static {0} {1} = {2};";
    public static string classStaticPrivatedMemberWithValue = @"private Static {0} {1} = {2};";

    public static string classConstPublicMember = @"public const {0} {1};";
    public static string classConstProtectedMember = @"protected const {0} {1};";
    public static string classConstPrivatedMember = @"private const {0} {1};";

    public static string classConstPublicMemberWithValue = @"public const {0} {1} = {2};";
    public static string classConstProtectedMemberWithValue = @"protected const {0} {1} = {2};";
    public static string classConstPrivatedMemberWithValue = @"private const {0} {1} = {2};";

    public static string enumHead = @"public enum {0}";
    public static string enumMember = @"{0},";
    public static string enumMemberWithValue = @"{0} = {1},";
    public static string enumMemberWithValueWithNote = @"{0} = {1}, // {2}";
}
