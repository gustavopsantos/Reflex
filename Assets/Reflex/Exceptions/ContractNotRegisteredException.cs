using System;
using System.Linq;
using Reflex.Extensions;

namespace Reflex.Exceptions
{
	internal sealed class ContractNotRegisteredException : Exception
	{
		public ContractNotRegisteredException(Type concrete, Type[] contracts) : base(GenerateMessage(concrete, contracts))
		{

		}


		private static string GenerateMessage(Type concrete, Type[] contracts)
		{
			return $"Concrete {concrete.GetFullName()} or one of these contracts [{string.Join(", ", contracts.Select(contract => contract.GetFullName()))}] was not registered and could not be removed.";
		}
	}
}