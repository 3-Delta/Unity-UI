using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileSplitter {
    public class Header {
        public int contentSize;
        public int filePathLength;
        public string filePath;

        public int SizeOf {
            get {
                return sizeof(int) + sizeof(int) + filePath.Length;
            }
        }

        public Header() { }
        public Header(int contentSize, int filePathLength, string filePath) {
            this.contentSize = contentSize;
            this.filePathLength = filePathLength;
            this.filePath = filePath;
        }

        public override string ToString() {
            return $"{contentSize}_{filePath}";
        }
    }

    public class Record {
        public class InterRecord {
            public string dst;
            public int startIndex;
            public int length;

            public InterRecord() { }
            public InterRecord(string dst, int startIndex, int length) {
                this.dst = dst;
                this.startIndex = startIndex;
                this.length = length;
            }

            public override string ToString() {
                return $"{dst}_{startIndex}_{length}";
            }
        }

        public string src;
        public List<InterRecord> dsts = new List<InterRecord>(0);

        public Record() { }

        public void AddRecord(string dst, int startIndex, int length) {
            InterRecord r = new InterRecord(dst, startIndex, length);
            dsts.Add(r);
        }
    }

    public class FileCombiner {
        public List<string> srcPaths = new List<string>();
        public string dstPath;
        // 1Mb
        // 限制过小，就是Split, 过大就是Combine
        public int SPLITSIZE = 1024 * 1024;

        public List<string> blockList = new List<string>();

        public virtual void Do() {
            blockList.Clear();

            int blockIndex = 0;
            int lastFileIndex = 0;
            List<byte> totalBytes = new List<byte>();
            List<byte> blcokBuffer = new List<byte>(SPLITSIZE);
            for (int i = 0, length = srcPaths.Count; i < length; ++i) {
                string srcPath = srcPaths[i];
                if (!File.Exists(srcPath)) {
                    continue;
                }

                byte[] fileBytes = File.ReadAllBytes(srcPath);
                totalBytes.AddRange(fileBytes);
                if (totalBytes.Count >= SPLITSIZE) {
                    while (totalBytes.Count >= SPLITSIZE) {
                        blcokBuffer.Clear();
                        blcokBuffer.AddRange(totalBytes);

                        Record record = new Record();
                    }
                }
                else { 

                }
                
            }
        }
    }

    public class ShellFileCombiner : FileCombiner {
        public const string BATCHFILE = "batch.bat";

        public string BatchCmd {
            get {
                string cmd = null;
                if (srcPaths != null) {
                    int length = srcPaths.Count;
                    if (length > 1) {
                        cmd = "copy /b ";
                        for (int i = 0; i < length - 1; ++i) {
                            cmd += $"\"{srcPaths[i]}\" + ";
                        }
                        cmd += $"\"{srcPaths[length - 1]}\" \"{dstPath}\"";
                    }
                    else if (length == 1) {
                        cmd = $"copy /b \"{srcPaths}\" \"{dstPath}\"";
                    }
                }
                return cmd;
            }
        }

        public void SaveToFIle() {
        }

        public override void Do() {
        }
    }
}
