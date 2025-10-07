#!/usr/bin/env python3
import os
import json
import argparse

def collect_cs_files(directories, recursive=False):
    cs_files = []
    for d in directories:
        if recursive:
            for root, _, files in os.walk(d):
                for f in files:
                    if f.endswith(".cs"):
                        cs_files.append(os.path.join(root, f))
        else:
            for f in os.listdir(d):
                if f.endswith(".cs"):
                    cs_files.append(os.path.join(d, f))
    return cs_files

def write_jsonl(cs_files, output_file):
    with open(output_file, "w", encoding="utf-8") as out:
        for path in cs_files:
            try:
                with open(path, "r", encoding="utf-8") as f:
                    content = f.read()
                record = {
                    "path": path,
                    "content": content
                }
                out.write(json.dumps(record, ensure_ascii=False) + "\n")
            except Exception as e:
                print(f"Skipping {path}: {e}")

def main():
    parser = argparse.ArgumentParser(description="Compile all C# files into one JSONL file.")
    parser.add_argument("directories", nargs="+", help="List of directories to scan for .cs files")
    parser.add_argument("-r", "--recursive", action="store_true", help="Recursively search subdirectories")
    parser.add_argument("-o", "--output", default="csharp_files.jsonl", help="Output JSONL file")
    args = parser.parse_args()

    cs_files = collect_cs_files(args.directories, args.recursive)
    print(f"Found {len(cs_files)} C# files.")
    write_jsonl(cs_files, args.output)
    print(f"Wrote {len(cs_files)} records to {args.output}")

if __name__ == "__main__":
    main()
