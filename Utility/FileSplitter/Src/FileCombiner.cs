using System;
using System.Collections.Generic;
using System.IO;

namespace FileSpliter {
    public class FileCombiner {
        public string recordFilePath;
        public List<string> splitedFiles = new List<string>();

        public FileCombiner(string recordFilePath, List<string> splitedFiles) {
            this.recordFilePath = recordFilePath;
            this.splitedFiles = splitedFiles;
        }

        public void Combine(string outputDir) {
            SplitRecords records = new SplitRecords();
            records.FromJson(recordFilePath);

            for (int i = 0, lengthI = records.recordList.Count; i < lengthI; i++) {
                var record = records.recordList[i];
                string src = record.src;
                for (int j = 0, lengthJ = record.dsts.Count; j < lengthJ; j++) {
                    var innerRecord = record.dsts[i];
                }
            }
        }
    }
}
