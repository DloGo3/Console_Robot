import json
import re


def update_erd_file(original_erd_path, json_path, output_erd_path):
    # Read JSON data
    with open(json_path, 'r') as json_file:
        json_data = json.load(json_file)

    # Create a dictionary for quick lookup of replacement values
    replacement_values = {item['Item1']: item['Item2']['posValue'] for item in json_data['Data']}
    # for key in replacement_values.keys():
    #     print(f"Key: {key}\tValue: {replacement_values[key]}")

    # Read the original ERD file content
    with open(original_erd_path, 'r', encoding='utf-8') as erd_file:
        erd_content = erd_file.read()

    # Replace CPOS points
    def replace_cpos(match):
        point_id = match.group(1)
        if point_id in replacement_values:
            values = replacement_values[point_id]
            new_pos_values = f'x={values[0]},y={values[1]},z={values[2]},a={values[3]},b={values[4]},c={values[5]},a7={values[6]},a8={values[7]},a9={values[8]},a10={values[9]},a11={values[10]},a12={values[11]},a13={values[12]},a14={values[13]},a15={values[14]},a16={values[15]}'
            # print(new_pos_values)
            return re.sub(r'x=.*?a16=[-.\d]+', new_pos_values, match.group(0))
        else:
            return match.group(0)

    # Replace APOS points
    def replace_apos(match):
        point_id = match.group(1)
        if point_id in replacement_values:
            values = replacement_values[point_id]
            new_pos_values = f'a1={values[0]},a2={values[1]},a3={values[2]},a4={values[3]},a5={values[4]},a6={values[5]},a7={values[6]},a8={values[7]},a9={values[8]},a10={values[9]},a11={values[10]},a12={values[11]},a13={values[12]},a14={values[13]},a15={values[14]},a16={values[15]}'
            # print(new_pos_values)
            return re.sub(r'a1=.*?a16=[-.\d]+', new_pos_values, match.group(0))
        else:
            return match.group(0)

    # Perform replacements
    erd_content = re.sub(r'(P\d+)=\{.*?x=.*?a16=[-.\d]+.*?\}', replace_cpos, erd_content, flags=re.DOTALL)
    erd_content = re.sub(r'(J\d+)=\{.*?a1=.*?a16=[-.\d]+.*?\}', replace_apos, erd_content, flags=re.DOTALL)

    # Write the updated content to the new ERD file
    with open(output_erd_path, 'w', encoding='utf-8') as output_file:
        output_file.write(erd_content)


# Example usage
original_erd_path = 'PX-ALLcurve-21.erd'
json_path = 'PX-ALLcurve-21.json'
output_erd_path = 'updated-PX-ALLcurve-21.erd'

update_erd_file(original_erd_path, json_path, output_erd_path)
