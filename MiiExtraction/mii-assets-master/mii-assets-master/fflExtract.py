import FFLiResource
from glbExporter import glbExporter
from textureImage import textureImage
import os

VERSION = "1.0.0"

# Define variables instead of using command-line arguments
resource_path = "C:/Users/jacob/Downloads/asset_model_character_mii_AFLResHigh_2_3_dat/asset/model/character/mii/AFLResHigh_2_3.dat"
texture_count = 365
shape_count = 859
shape_export_dir = "C:/Users/jacob/Desktop/Mii/Shapes"
texture_export_dir = "C:/Users/jacob/Desktop/Mii/Textures"

def print_help():
    print("\n".join([
        "",
        "===========================",
        "fflExtract.py version " + str(VERSION),
        "===========================",
        "Extract resource files used by Nintendo's Mii Face Library",
        "By Jaames (github.com/jaames)",
        "FFL archive structure reverse-engineered by Kinnay (https://github.com/Kinnay)",
        "",
        "Usage:",
        "======",
        "This script is now using predefined variables for paths and counts.",
        "",
        "Issues:",
        "=======",
        "If you find any bugs in this script, please report them here:",
        "https://github.com/jaames/mii/issues",
        ""
    ]))

if __name__ == "__main__":
    if not resource_path:
        print("Error: no input file specified")
        print_help()
        exit()

    if all([resource_path, texture_count, shape_count]) and any([shape_export_dir, texture_export_dir]):
        with open(resource_path, "rb") as ffl:
            # Directly instantiate FFLiResource instead of using parse
            res = FFLiResource.FFLiResource(ffl, texture_count, shape_count)

            # Extract meshes
            if shape_export_dir:
                print("Extracting meshes...")
                os.makedirs(shape_export_dir, exist_ok=True)
                for index in range(len(res.shapes)):
                    shape = res.get_shape(index)
                    if shape:
                        shape_path = os.path.join(shape_export_dir, f"shape_{index}.glb")
                        glb = glbExporter()
                        verts = shape.get_verts()[["x", "y", "z"]]
                        uvs = shape.get_tex_coords()
                        colors = shape.get_vert_colors()
                        faces = shape.get_faces()
                        glb.add_verts(verts)
                        glb.add_faces(faces)
                        if len(uvs) > 0:
                            glb.add_tex_coords(uvs)
                        if len(colors) > 1:
                            glb.add_vert_colors(colors)
                        glb.save(shape_path)
                print("Done!")

            # Extract textures
            if texture_export_dir:
                print("Extracting textures...")
                os.makedirs(texture_export_dir, exist_ok=True)
                for index in range(len(res.textures)):
                    texture = res.get_texture(index)
                    if texture:
                        texture_path = os.path.join(texture_export_dir, f"tex_{index}.png")
                        image = textureImage(texture.width, texture.height, texture.format)
                        image.set_pixels(texture.get_pixels())
                        image.save(texture_path)
                print("Done!")
