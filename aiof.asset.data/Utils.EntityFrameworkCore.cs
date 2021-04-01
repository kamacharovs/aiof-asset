using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace aiof.asset.data
{
    public static class UtilsEntityFrameworkCore
    {
        public static PropertyBuilder HasSnakeCaseColumnName(this PropertyBuilder propertyBuilder)
        {
            propertyBuilder.Metadata.SetColumnName(
                propertyBuilder
                    .Metadata
                    .Name
                    .ToSnakeCase());

            return propertyBuilder;
        }
    }
}
