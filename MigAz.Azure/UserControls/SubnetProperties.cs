﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MigAz.Azure.MigrationTarget;
using MigAz.Core.ArmTemplate;

namespace MigAz.Azure.UserControls
{
    public partial class SubnetProperties : UserControl
    {
        private TargetTreeView _TargetTreeView;
        private MigrationTarget.Subnet _Subnet;
        private bool _IsBinding = false;

        public delegate Task AfterPropertyChanged();
        public event AfterPropertyChanged PropertyChanged;

        public SubnetProperties()
        {
            InitializeComponent();
        }

        internal void Bind(TargetTreeView targetTreeView, MigrationTarget.Subnet targetSubnet)
        {
            try
            {
                _IsBinding = true;
                _TargetTreeView = targetTreeView;
                _Subnet = targetSubnet;

                txtTargetName.Text = targetSubnet.TargetName;

                if (targetSubnet.SourceSubnet != null)
                {
                    if (targetSubnet.SourceSubnet.GetType() == typeof(Azure.Asm.Subnet))
                    {
                        Asm.Subnet asmSubnet = (Asm.Subnet)targetSubnet.SourceSubnet;

                        lblSourceName.Text = asmSubnet.Name;
                        lblAddressSpace.Text = asmSubnet.AddressPrefix;
                    }
                    else if (targetSubnet.SourceSubnet.GetType() == typeof(Azure.Arm.Subnet))
                    {
                        Arm.Subnet armSubnet = (Arm.Subnet)targetSubnet.SourceSubnet;

                        lblSourceName.Text = armSubnet.Name;
                        lblAddressSpace.Text = armSubnet.AddressPrefix;
                    }
                }

                if (String.Compare(txtTargetName.Text, ArmConst.GatewaySubnetName, true) == 0)
                {
                    // if gateway subnet, the name can't be changed
                    txtTargetName.Enabled = false;
                }

                networkSecurityGroup.Bind(_Subnet.NetworkSecurityGroup, _TargetTreeView);
                routeTable.Bind(_Subnet.RouteTable, _TargetTreeView);
            }
            finally
            {
                _IsBinding = false;
            }
        }

        private void txtTargetName_TextChanged(object sender, EventArgs e)
        {
            TextBox txtSender = (TextBox)sender;

            _Subnet.SetTargetName(txtSender.Text, _TargetTreeView.TargetSettings);

            if (!_IsBinding)
                PropertyChanged?.Invoke();
        }

        private void txtTargetName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
