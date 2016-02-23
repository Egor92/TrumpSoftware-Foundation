using System.Collections.Generic;
using System.Linq;
using TrumpSoftware.Common.Exceptions;

#if WPF
namespace TrumpSoftware.Wpf.Converters.Enums
#elif WINRT
namespace TrumpSoftware.WinRT.Converters.Enums
#endif
{
    public enum BinaryLogicalOperation
    {
        And,
        Or,
    }

    internal static class BinaryLogicalOperationExtensions
    {
        internal static bool ApplyOperationFor(this BinaryLogicalOperation operation, IEnumerable<bool> values)
        {
            switch (operation)
            {
                case BinaryLogicalOperation.And:
                    return values.Aggregate(true, (x1, x2) => x1 && x2);
                case BinaryLogicalOperation.Or:
                    return values.Aggregate(false, (x1, x2) => x1 || x2);
                default:
                    throw new UnhandledCaseException(operation);
            }
        }
    }
}
