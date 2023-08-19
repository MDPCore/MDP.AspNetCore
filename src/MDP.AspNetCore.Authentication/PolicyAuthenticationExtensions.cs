using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace MDP.AspNetCore.Authentication
{
    public static class PolicyAuthenticationExtensions
    {
        // Methods
        public static AuthenticationBuilder AddPolicy(this AuthenticationBuilder builder, PolicyAuthenticationSetting authenticationSetting = null)
        {
            #region Contracts

            if (builder == null) throw new ArgumentException(nameof(builder));

            #endregion

            // AddPolicy
            return builder.AddPolicy(PolicyAuthenticationDefaults.AuthenticationScheme, authenticationSetting);
        }

        public static AuthenticationBuilder AddPolicy(this AuthenticationBuilder builder, string authenticationScheme, PolicyAuthenticationSetting authenticationSetting = null)
        {
            #region Contracts

            if (builder == null) throw new ArgumentException(nameof(builder));
            if (string.IsNullOrEmpty(authenticationScheme) == true) throw new ArgumentException(nameof(authenticationScheme));

            #endregion

            // AuthenticationSetting
            if (authenticationSetting == null) authenticationSetting = new PolicyAuthenticationSetting();
            if (string.IsNullOrEmpty(authenticationSetting.DefaultScheme) == true) throw new InvalidOperationException($"{nameof(authenticationSetting.DefaultScheme)}=null");

            // PolicyScheme
            builder.AddPolicyScheme(authenticationScheme, null, authenticationOptions =>
            {
                // ForwardDefaultSelector
                authenticationOptions.ForwardDefaultSelector = context =>
                {
                    // PolicyAuthenticationSelectorList
                    var policyAuthenticationSelectorList = context.RequestServices.GetRequiredService<IList<PolicyAuthenticationSelector>>();
                    if (policyAuthenticationSelectorList == null) throw new InvalidOperationException($"{nameof(policyAuthenticationSelectorList)}=null");

                    // PolicyAuthenticationSelector
                    foreach (var policyAuthenticationSelector in policyAuthenticationSelectorList)
                    {
                        // Check
                        if (policyAuthenticationSelector.Check(context) == false) continue;

                        // Apply
                        return policyAuthenticationSelector.AuthenticationScheme;
                    }

                    // DefaultScheme
                    return authenticationSetting.DefaultScheme;
                };
            });

            // Return
            return builder;
        }
    }
}
