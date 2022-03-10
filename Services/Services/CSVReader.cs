using System;
using System.IO;
using System.Collections.Generic;
using Core.Models.Objects;
using Microsoft.Extensions.Logging;

namespace Services.Services
{
    public class CSVReader
    {

        public Output ReadCsvFile(InputFromBody input)
        {
            Output output = new();
            output.Postcodes = new List<string>();
            output.Uids = new List<string>();
            var pcs = output.Postcodes;
            var uids = output.Uids;

            var pcValues = input.PostCodes[0].Split(',');
            var uidsValues = input.Uids[0].Split(',');

            // Check the length of the values array and throw exception if provided array probably doesn't contain anything
            if (((pcValues.Length == 0 || pcValues.Length == 1) && (string.IsNullOrWhiteSpace(pcValues[0]) || string.IsNullOrEmpty(pcValues[0]))))
                throw new Exception("No postcode values provided");

            // Go through each array
            for (var i = 0; i < pcValues.Length; i++)
            {
                pcs.Add(pcValues[i]);
                uids.Add(uidsValues[i]);
            }

            if (pcs.Count > 0) return output;
            throw new Exception("Unknown error: could not process");
        }
    }
}
