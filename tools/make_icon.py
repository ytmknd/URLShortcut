"""Generate the app icon from simple vector-like shapes with Pillow."""

from pathlib import Path

from PIL import Image, ImageDraw


ROOT = Path(__file__).resolve().parents[1]
OUTPUT = ROOT / "assets" / "UrlShortcut.ico"
SIZE = 1024

image = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
mask = Image.new("L", (SIZE, SIZE))
ImageDraw.Draw(mask).rounded_rectangle((40, 40, 984, 984), radius=220, fill=255)

for y in range(SIZE):
    top = (14, 113, 205)
    bottom = (5, 63, 150)
    ratio = y / (SIZE - 1)
    color = tuple(round(a * (1 - ratio) + b * ratio) for a, b in zip(top, bottom))
    ImageDraw.Draw(image).line((0, y, SIZE, y), fill=(*color, 255))
image.putalpha(mask)


def link(center_x: int, center_y: int) -> Image.Image:
    layer = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    draw = ImageDraw.Draw(layer)
    outer = (center_x - 260, center_y - 135, center_x + 260, center_y + 135)
    inner = (center_x - 172, center_y - 53, center_x + 172, center_y + 53)
    draw.rounded_rectangle(outer, radius=130, fill="white")
    draw.rounded_rectangle(inner, radius=53, fill=(0, 0, 0, 0))
    return layer.rotate(42, resample=Image.Resampling.BICUBIC, center=(center_x, center_y))


image.alpha_composite(link(400, 620))
image.alpha_composite(link(624, 404))

OUTPUT.parent.mkdir(parents=True, exist_ok=True)
image.save(OUTPUT, format="ICO", sizes=[(16, 16), (24, 24), (32, 32), (48, 48), (64, 64), (128, 128), (256, 256)])
image.resize((256, 256), Image.Resampling.LANCZOS).save(OUTPUT.with_suffix(".png"))
print(OUTPUT)
