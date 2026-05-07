import re
import os

with open('/home/analista/Escritorio/SobrevivienteCallejero/Codigo_Megacompleto.txt', 'r') as f:
    lines = f.readlines()

current_file = None
content = []

for line in lines:
    match = re.search(r'--- ARCHIVO: \./(.*?) ---', line)
    if match:
        if current_file:
            with open('/home/analista/Escritorio/SobrevivienteCallejero/' + current_file, 'w') as out:
                out.writelines(content)
        current_file = match.group(1).strip()
        content = []
    elif line.strip() == '-e':
        if current_file:
            with open('/home/analista/Escritorio/SobrevivienteCallejero/' + current_file, 'w') as out:
                out.writelines(content)
        current_file = None
        content = []
    elif current_file:
        # Avoid including any trailing whitespace issues or the `line` format numbers
        content.append(line)

if current_file:
    with open('/home/analista/Escritorio/SobrevivienteCallejero/' + current_file, 'w') as out:
        out.writelines(content)

print("Files successfully extracted.")
