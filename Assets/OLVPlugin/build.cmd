@echo off

: set GHS_ROOT=D:/devroot/bamboo/env/env_sdk2.08/ghs/multi5315
: set CAFE_ROOT=D:/devroot/bamboo/env/env_sdk2.08/cafe_sdk

set B=build
set CC=%GHS_ROOT%/cxppc.exe
set LINK=%GHS_ROOT%/cxppc.exe
set PREPRPL=%CAFE_ROOT%/system/bin/tool/preprpl64.exe
set MAKERPL=%CAFE_ROOT%/system/bin/tool/makerpl64.exe
set CCFLAGS=-Ogeneral -c -cpu=espresso -c99 --g++ -kanji=shiftjis -sda=none -X332 --no_implicit_include -G -dbg_source_root "." --no_commons -w --no_exceptions
set CCINCLUDES=%CAFE_ROOT%/system/include .
set CCDEFINES=-DNDEV=1 -DCAFE=2 -DPLATFORM=CAFE -DEPPC -DWEBPLUG=0 -DFT2_BUILD_LIBRARY -DUNITTEST_FORCE_NO_EXCEPTIONS -DUNITY_RELEASE=1 -DMASTER_BUILD=1 -DENABLE_PROFILER=0

set OUTNAME=OLVPlugin
set OUTNAME_EXPORT=%OUTNAME%.export.o
set OBJS=%B%/main.o

set R=%~dp0
pushd %R%

@echo on
mkdir build

%CC% %CCFLAGS% -I %CCINCLUDES% %CCDEFINES% -o %B%/main.o main.cpp
%PREPRPL% -xall -log %B%/%OUTNAME%.log -e __rpl_crt -o %B%/%OUTNAME_EXPORT% %B%/main.o

set LIBS=-lnsysnet.a -lnn_ac.a -lnn_olv.a -lnlibcurl.a
set LIBS=-lnsysnet.a -lnn_ac.a -lnn_olv.a -lnlibcurl.a -lnn_ngc.a -lnn_dbg.a -lnn_nstd.a -lnn_os.a -lnn_nstd.a -lsci.a -lnn_act.a

%CC% %B%/%OUTNAME_EXPORT% %OBJS% -e __rpl_crt -relprog_cafe %CAFE_ROOT%/system/include/cafe/eppc.Cafe.rpl.ld -map -sda=none -nostartfile -L %CAFE_ROOT%/system/lib/ghs/cafe/NDEBUG -lrpl.a -lcoredyn.a %LIBS% -o %B%/%OUTNAME%.elf
%MAKERPL% -t BUILD_TYPE=NDEBUG -zx -checknosda -nolib %B%/%OUTNAME%.elf

popd
