import subprocess
import shutil
from pathlib import Path

ROOT_DIR = Path(__file__).resolve().parent.parent      # CoreKit/
SRC_DIR = ROOT_DIR / "src"                             # CoreKit/src
DEST_DIR = ROOT_DIR / "artifacts" / "v1.0.0"            # CoreKit/packages/v1.0.0
PACK_SOLUTION = SRC_DIR / "CoreKit.Pack.sln"          # CoreKit/CoreKit.Pack.sln

def build_projects():
    if not PACK_SOLUTION.exists():
        raise FileNotFoundError(f"❌ Solution file not found: {PACK_SOLUTION}")
    
    print(f"\n🔧 Building solution: {PACK_SOLUTION.name}")
    result = subprocess.run(["dotnet", "build", str(PACK_SOLUTION), "-c", "Release"])
    
    if result.returncode != 0:
        raise RuntimeError("❌ Build failed. Check output above.")

def pack_projects():
    print("\n📦 Packing projects...")
    csproj_files = list(SRC_DIR.rglob("*.csproj"))

    if not csproj_files:
        raise FileNotFoundError("❌ No .csproj files found.")

    for csproj in csproj_files:
        print(f"→ Packing: {csproj}")
        result = subprocess.run(["dotnet", "pack", str(csproj), "-c", "Release"], capture_output=True)
        
        if result.returncode != 0:
            print(result.stdout.decode())
            print(result.stderr.decode())
            raise RuntimeError(f"❌ Failed to pack {csproj.name}")

def collect_packages():
    print("\n📂 Collecting .nupkg files...")
    DEST_DIR.mkdir(parents=True, exist_ok=True)

    found = False
    for nupkg_file in SRC_DIR.rglob("bin/Release/**/*.nupkg"):
        print(f"✔ Moving: {nupkg_file.name}")
        shutil.copy(nupkg_file, DEST_DIR / nupkg_file.name)
        found = True

    if not found:
        print("⚠️ No .nupkg files were found.")
    else:
        print(f"\n✅ All packages collected in: {DEST_DIR}")

def main():
    try:
        build_projects()
        pack_projects()
        collect_packages()
    except Exception as e:
        print(f"\n🚨 Error: {e}")

if __name__ == "__main__":
    main()
