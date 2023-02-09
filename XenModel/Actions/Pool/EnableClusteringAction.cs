/* Copyright (c) Cloud Software Group, Inc. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using XenAPI;

namespace XenAdmin.Actions
{
    public class EnableClusteringAction : AsyncAction
    {
        private XenAPI.Network network;
        
        public EnableClusteringAction(Pool pool, XenAPI.Network network)
            : base(pool.Connection, Messages.ENABLE_CLUSTERING_ON_POOL,
            string.Format(Messages.ENABLING_CLUSTERING_ON_POOL, pool.Name()), true)
        {
            this.network = network;
            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("cluster.pool_create");
            ApiMethodsToRoleCheck.Add("pif.set_disallow_unplug");
            #endregion
        }

        protected override void Run()
        {
            foreach (var pif in Connection.ResolveAll(network.PIFs))
            {
                PIF.set_disallow_unplug(Session, pif.opaque_ref, true);
            }
            var cluster = new Cluster(); // this Cluster object is only used for getting the default values for token_timeout and token_timeout_coefficient
            Cluster.pool_create(Session, network.opaque_ref, "corosync", cluster.token_timeout, cluster.token_timeout_coefficient);
            Description = string.Format(Messages.ENABLED_CLUSTERING_ON_POOL, Pool.Name());
            
        }
    }
}
