using System;
using System.Collections.Generic;
using System.IO;

namespace FileSpliter {
    [Serializable]
    public class SplitRecord {
        [Serializable]
        public class InterRecord {
            public string fileName;
            public int startIndex;
            public int length;

            public InterRecord() { }
            public InterRecord(int blockIndex, int startIndex, int length) {
                this.fileName = FileSpliter.SplitedFileName(blockIndex);
                this.startIndex = startIndex;
                this.length = length;
            }

            public override string ToString() {
                return $"{fileName}_{startIndex}_{length}";
            }
        }

        public string src;
        public List<InterRecord> dsts = new List<InterRecord>(0);

        public SplitRecord() { }
        public SplitRecord(string src) {
            this.src = src;
            dsts.Clear();
        }

        public InterRecord AddRecord(int blockIndex, int startIndex, int length) {
            InterRecord r = new InterRecord(blockIndex, startIndex, length);
            dsts.Add(r);
            return r;
        }
    }

    [Serializable]
    public class SplitRecords : DefaultJsonSerialize {
        public List<SplitRecord> recordList = new List<SplitRecord>(0);

        public SplitRecords() {
            recordList.Clear();
        }

        public SplitRecord AddRecord(string src) {
            SplitRecord r = new SplitRecord(src);
            recordList.Add(r);
            return r;
        }
    }

    public class FileSpliter {
        public const string BLOCKNAME = "SplitedBlock.{0}";

        public List<string> srcFilePaths = new List<string>();
        // 1Mb
        // 限制过小，就是Split, 过大就是Combine
        public int SPLITSIZE = 1024 * 1024;

        private SplitRecords records;

        public FileSpliter(List<string> srcFilePaths, int SPLITSIZE) {
            this.srcFilePaths = srcFilePaths;
            this.SPLITSIZE = SPLITSIZE;
        }

        public static string SplitedFileName(int index) {
            return string.Format(BLOCKNAME, index.ToString());
        }

        public void Split(string outputDir) {
            records = new SplitRecords();
            int blockFileIndex = 0;

            byte[] blcokBuffer = new byte[SPLITSIZE];
            int blockStartIndex = 0;
            for (int i = 0, length = srcFilePaths.Count; i < length; ++i) {
                string srcPath = srcFilePaths[i];
                if (!File.Exists(srcPath)) {
                    continue;
                }

                byte[] fileBytes = File.ReadAllBytes(srcPath);
                if (fileBytes.Length + blockStartIndex == SPLITSIZE) {
                    Array.Copy(fileBytes, 0, blcokBuffer, blockStartIndex, fileBytes.Length);

                    SplitRecord record = records.AddRecord(srcPath);
                    record.AddRecord(blockFileIndex, blockStartIndex, fileBytes.Length);
                    OnBlock(blockFileIndex, blcokBuffer, outputDir);

                    blockFileIndex++;
                    blockStartIndex = 0;
                }
                else if (fileBytes.Length + blockStartIndex < SPLITSIZE) {
                    Array.Copy(fileBytes, 0, blcokBuffer, blockStartIndex, fileBytes.Length);

                    SplitRecord record = records.AddRecord(srcPath);
                    record.AddRecord(blockFileIndex, blockStartIndex, fileBytes.Length);

                    // 不足
                    if (i >= length - 1) {
                        OnBlock(blockFileIndex, blcokBuffer, outputDir);
                    }

                    blockStartIndex = blockStartIndex + fileBytes.Length;
                }
                else {
                    int fileStartIndex = 0;
                    int fileRemain = 0;
                    int fileLength = fileBytes.Length;
                    while (fileLength + blockStartIndex > SPLITSIZE) {
                        fileRemain = SPLITSIZE - blockStartIndex;
                        Array.Copy(fileBytes, fileStartIndex, blcokBuffer, blockStartIndex, fileRemain);

                        fileStartIndex += fileRemain;
                        SplitRecord record = records.AddRecord(srcPath);
                        record.AddRecord(blockFileIndex, blockStartIndex, fileRemain);
                        OnBlock(blockFileIndex, blcokBuffer, outputDir);

                        fileLength -= fileRemain;
                        blockFileIndex++;
                        blockStartIndex = 0;
                    }

                    fileRemain = fileBytes.Length - fileStartIndex;
                    if (fileRemain > 0) {
                        Array.Copy(fileBytes, fileStartIndex, blcokBuffer, blockStartIndex, fileRemain);
                        SplitRecord record = records.AddRecord(srcPath);
                        record.AddRecord(blockFileIndex, blockStartIndex, fileRemain);

                        blockFileIndex++;
                        blockStartIndex += fileRemain;
                    }
                }
            }

            string recordPath = $"{outputDir}/BlockList.json";
            records.ToJson(recordPath);
        }

        private void OnBlock(int blockFileIndex, byte[] blcokBuffer, string outputDir) {
            string fileName = FileSpliter.SplitedFileName(blockFileIndex);
            string filePath = $"{outputDir}/{fileName}";
            if (File.Exists(filePath)) {
                File.Delete(filePath);
            }
            using (var fi = File.Create(filePath)) {
            }
            File.WriteAllBytes(filePath, blcokBuffer);
        }
    }
}
