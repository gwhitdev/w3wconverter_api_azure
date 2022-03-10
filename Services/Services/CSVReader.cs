using System;
using System.IO;
using System.Collections.Generic;
using Core.Models.Objects;
using Microsoft.Extensions.Logging;

namespace Services.Services
{
    public class CSVReader
    {
        private InputFromBody _input;

        public CSVReader(InputFromBody input)
        {
            _input = input;
        }

        public ListOfPostcodes ReadCsv()
        {
            ListOfPostcodes list = new();
            list.Postcodes = new();
            list.Uids = new();
            var pcs = list.Postcodes;
            var uids = list.Uids;

            var pcValues = _input.PostCodes[0].Split(',');
            var uidsValues = _input.Uids[0].Split(',');

            // Check the length of the values array and throw exception if provided array probably doesn't contain anything
            if (((pcValues.Length == 0 || pcValues.Length == 1) && (string.IsNullOrWhiteSpace(pcValues[0]) || string.IsNullOrEmpty(pcValues[0]))))
                throw new Exception("No postcode values provided");

            // Go through each array
            for (var i = 0; i < pcValues.Length; i++)
            {
                pcs.Add(pcValues[i]);
                uids.Add(uidsValues[i]);
            }

            if (pcs.Count > 0) return list;
            throw new Exception("Unknown error: could not process");
        }
    }
}
