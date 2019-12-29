using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetFwTypeLib;

using Ports.Configuration;

namespace Ports.Firewall
{
    public sealed class FirewallManager
    {

        private readonly INetFwPolicy2 _firewallPolicy;

        private FirewallManager()
        {
            _firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
        }

        private static FirewallManager _instance;

        public static FirewallManager Instance
        {
            get
            {
                return _instance ?? (_instance = new FirewallManager());
            }
        }

        public void Toggle(EntryConfig entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            var entryName = entry.Name.ToLowerInvariant();

            if (entryName == "domain")
            {
                ToggleProfileRule(NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_DOMAIN);
            }
            else if (entryName == "private")
            {
                ToggleProfileRule(NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PRIVATE);
            }
            else if (entryName == "public")
            {
                ToggleProfileRule(NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PUBLIC);
            }
            else
            {
                ToggleRules(entry.Rules);
            }
        }

        public bool IsEnabled(EntryConfig entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            var entryName = entry.Name.ToLowerInvariant();

            if (entryName == "domain")
            {
                return _firewallPolicy.DefaultOutboundAction[NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_DOMAIN] == NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            }
            else if (entryName == "private")
            {
                return _firewallPolicy.DefaultOutboundAction[NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PRIVATE] == NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            }
            else if (entryName == "public")
            {
                return _firewallPolicy.DefaultOutboundAction[NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PUBLIC] == NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            }
            else
            {
                return entry.Rules.All(rule => _firewallPolicy.Rules.Item(rule).Enabled);
            }
        }

        private void ToggleProfileRule(NET_FW_PROFILE_TYPE2_ profileType)
        {
            if (_firewallPolicy.DefaultOutboundAction[profileType] == NET_FW_ACTION_.NET_FW_ACTION_BLOCK)
            {
                _firewallPolicy.DefaultOutboundAction[profileType] = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            }
            else
            {
                _firewallPolicy.DefaultOutboundAction[profileType] = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
            }
        }

        private void ToggleRules(IEnumerable<string> rules)
        {
            foreach (var name in rules)
            {
                var rule = _firewallPolicy.Rules.Item(name);
                rule.Enabled = !rule.Enabled;
            }
        }
    }
}
