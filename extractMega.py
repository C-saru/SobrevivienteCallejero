import argparse
import re
import os
import sys

def main():
    parser = argparse.ArgumentParser(description="Extract files from a combined text file.")
    parser.add_argument("input_file", nargs='?', default="Codigo_Megacompleto.txt", help="Path to the input combined text file.")
    parser.add_argument("-o", "--output-dir", default=".", help="Directory to extract files into.")
    args = parser.parse_args()

    input_file = args.input_file
    output_dir = os.path.abspath(args.output_dir)

    if not os.path.isfile(input_file):
        print(f"Error: Input file '{input_file}' does not exist.")
        sys.exit(1)

    if not os.path.exists(output_dir):
        os.makedirs(output_dir, exist_ok=True)

    with open(input_file, 'r', encoding='utf-8') as f:
        lines = f.readlines()

    current_file = None
    content = []

    def save_current_file():
        if current_file:
            target_path = os.path.abspath(os.path.join(output_dir, current_file))
            if os.path.commonpath([output_dir, target_path]) != output_dir:
                print(f"Warning: Skipping file '{current_file}' due to path traversal attempt.")
                return

            os.makedirs(os.path.dirname(target_path), exist_ok=True)
            with open(target_path, 'w', encoding='utf-8') as out:
                out.writelines(content)

    for line in lines:
        match = re.search(r'--- ARCHIVO: \./(.*?) ---', line)
        if match:
            save_current_file()
            current_file = match.group(1).strip()
            content = []
        elif line.strip() == '-e':
            save_current_file()
            current_file = None
            content = []
        elif current_file:
            # Avoid including any trailing whitespace issues or the `line` format numbers
            content.append(line)

    save_current_file()

    print("Files successfully extracted.")

if __name__ == "__main__":
    main()
