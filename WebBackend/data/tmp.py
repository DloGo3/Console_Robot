def remove_empty_lines(file_path):
    with open(file_path, 'r') as f:
        lines = f.readlines()

    non_empty_lines = [line for line in lines if line.strip() != ""]

    with open(file_path, 'w') as f:
        f.writelines(non_empty_lines)


if __name__ == '__main__':
    file_path = "ZD-UPCVMOD-21.erd"  # Replace with your file path
    remove_empty_lines(file_path)
