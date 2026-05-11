import re
import os
import argparse

def sanitize_path(base_dir, relative_path):
    """
    Ensures the path is normalized and doesn't escape base_dir.
    """
    abs_base = os.path.abspath(base_dir)
    # Join and normalize to resolve '..' etc.
    target_path = os.path.abspath(os.path.join(abs_base, relative_path))

    # Check if the target_path starts with abs_base
    if os.path.commonpath([abs_base, target_path]) == abs_base:
        return target_path
    else:
        raise ValueError(f"Security Warning: Path traversal detected and blocked: {relative_path}")

def main():
    parser = argparse.ArgumentParser(description="Extract files from a mega-file securely.")
    parser.add_argument("-i", "--input", default="Codigo_Megacompleto.txt",
                        help="Input file (default: Codigo_Megacompleto.txt)")
    parser.add_argument("-o", "--output", default=".",
                        help="Output directory (default: current directory)")
    args = parser.parse_args()

    if not os.path.exists(args.input):
        print(f"Error: Input file '{args.input}' not found.")
        return

    # Ensure output directory exists
    try:
        os.makedirs(args.output, exist_ok=True)
    except Exception as e:
        print(f"Error creating output directory: {e}")
        return

    with open(args.input, 'r') as f:
        lines = f.readlines()

    current_file = None
    content = []

    def save_current_file():
        if current_file:
            try:
                out_path = sanitize_path(args.output, current_file)
                os.makedirs(os.path.dirname(out_path), exist_ok=True)
                with open(out_path, 'w') as out:
                    out.writelines(content)
                print(f"Extracted: {current_file}")
            except ValueError as e:
                print(e)
            except Exception as e:
                print(f"Error writing to {current_file}: {e}")

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
            content.append(line)

    save_current_file()
    print("Extraction process completed.")

if __name__ == "__main__":
    main()
