using Jotunn.Configs;
using System.Collections.Generic;
using System.Linq;


namespace MVBP.Configs
{
    /// <summary>
    ///     Class to handle serializing and de-serializing piece requirements.
    /// </summary>
    internal static class RequirementsEntry
    {
        private const char reqSep = ';';
        private const char amountSep = ',';

        /// <summary>
        ///     Deserialize requirements string into ReqData
        /// </summary>
        /// <param name="reqString"></param>
        /// <returns></returns>
        public static List<RequirementConfig> Deserialize(string reqString)
        {
            // avoid calling Trim() on null object
            if (string.IsNullOrWhiteSpace(reqString))
            {
                return new List<RequirementConfig>() { new RequirementConfig() { Item = " ", Amount = 0} };
            }

            // If not empty
            List<RequirementConfig> requirements = new();

            foreach (string entry in reqString.Split(reqSep))
            {
                string[] values = entry.Split(amountSep);

                var reqData = new RequirementConfig()
                {
                    Item = values[0].Trim(),
                    Amount = values.Length > 1 && int.TryParse(values[1], out int amount) ? amount : 1
                };
                requirements.Add(reqData);
            }
            return requirements;
        }

        /// <summary>
        ///     Convert requirements string from cfg file to Piece.Requirement Array
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static Piece.Requirement[] CreateRequirementsArray(string reqString)
        {
            var reqsData = Deserialize(reqString);
            var requirements = new List<Piece.Requirement>();
            foreach (RequirementConfig reqData in reqsData)
            {
                if (reqData.Item == " ")
                {
                    continue;
                }

                var itm = ObjectDB.instance.GetItemPrefab(reqData.Item)?.GetComponent<ItemDrop>();
                if (itm == null)
                {
                    Log.LogWarning($"Unable to find requirement ID: {reqData.Item}");
                    continue;
                }
                Piece.Requirement pieceReq = new()
                {
                    m_resItem = itm,
                    m_amount = reqData.Amount,
                    m_recover = true
                };
                requirements.Add(pieceReq);
            }
            return requirements.ToArray();
        }

        public static string Serialize(IEnumerable<RequirementConfig> reqs)
        {
            return string.Join($"{reqSep}", reqs.Select(r => $"{r.Item}{amountSep}{r.Amount}"));
        }
    }
}
