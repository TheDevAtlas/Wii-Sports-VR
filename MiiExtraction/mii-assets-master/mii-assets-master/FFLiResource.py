import numpy as np
from struct import unpack
from io import BytesIO
import zlib

import FFLiResourceTexture
import FFLiResourceShape

class FFLiResource:
    def __init__(self, stream, texture_count, shape_count):
        self.stream = stream
        self.magic, self.version, self.uncompressed_size = unpack(">II4xI4x", stream.read(0x14))
        self.texture_buffer_sizes = np.fromstring(self.stream.read(0x2C), dtype=">u4")
        self.textures = self.read_parts_info(texture_count)
        self.shape_buffer_sizes = np.fromstring(self.stream.read(0x30), dtype=">u4")
        self.shapes = self.read_parts_info(shape_count)

    def read_parts_info(self, count):
        dt = np.dtype([
            ("offset", ">u4"),
            ("uncompressed_size", ">u4"),
            ("compressed_size", ">u4"),
            ("unknown1", "u1"),
            ("flags", "u1"),
            ("unknown2", "u1"),
            ("compression", "u1"),
        ])
        return np.fromstring(self.stream.read(count * 16), dtype=dt)

    def get_resource_data(self, type, index):
        resource = self.shapes[index] if type == "shape" else self.textures[index]
        offset = resource["offset"]
        if offset == 0:
            return None
        else:
            self.stream.seek(offset)
            data = self.stream.read(resource["uncompressed_size"])
            if resource["compression"] == 5 or resource["compression"] > 100:
                return data
            else:
                return zlib.decompress(data)

    def get_texture(self, index):
        data = self.get_resource_data("texture", index)
        if not data:
            return None
        else:
            return FFLiResourceTexture.FFLiResourceTexture(BytesIO(data))

    def get_shape(self, index):
        data = self.get_resource_data("shape", index)
        if not data:
            return None
        else:
            return FFLiResourceShape.FFLiResourceShape(BytesIO(data))

    @classmethod
    def parse(cls, stream, texture_count, shape_count):
        return cls(stream, texture_count, shape_count)
