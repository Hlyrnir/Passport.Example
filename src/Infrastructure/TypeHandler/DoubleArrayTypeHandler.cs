using Dapper;
using System.Data;
using System.Text.Json;

namespace Infrastructure.TypeHandler
{
	internal sealed class DoubleArrayTypeHandler : SqlMapper.TypeHandler<double[]>
	{
		public override double[]? Parse(object oData)
		{
			if (oData is string sJson)
				return JsonSerializer.Deserialize<double[]>(sJson);

			return null;
		}

		public override void SetValue(IDbDataParameter dpParameter, double[]? dArray)
		{
			dpParameter.Value = JsonSerializer.Serialize(dArray);
			dpParameter.DbType = DbType.String;
		}
	}
}
