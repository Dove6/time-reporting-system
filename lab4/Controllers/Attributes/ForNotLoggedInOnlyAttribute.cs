namespace Trs.Controllers.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class ForNotLoggedInOnlyAttribute : Attribute
{
}
