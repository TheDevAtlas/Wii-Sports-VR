#include <string>
#include <cstdio>

#include <cafe/os.h>
#include <cafe/mem.h>
#include <cafe/nssl/nsslclient.h>
#include <nn/olv.h>
#include <nn/ngc.h>
#include <nn/ffl/FFLStandard.h>
#include <nn/act/act_Api.h>
#include <nn/erreula/erreula_Const.h>


#include "../UnityInterface.h"

/*
Plugin showing simple interaction with Miiverse via OLV library

Plugin functions:

OLV_Init()                      ~ initializes olv (and ssl). Networking libs (ac and so) are considered to be already initialised which is the case in unity.
OLV_PostText(char* szMessage)   ~ sends a post to Miiverse community of the application using the string parameter as text message.
char* OLV_GetLatestPostedText() ~ receives latest post and returns its text message. Note that it takes some time that the Miiverse server will be up-to-date
OLV_Finalize()                  ~ shutdown of olv (and ssl).

See documentation on OLV lib and the samples under cafe_sdk\system\src\demo\nn\olv\sample for more ideas to access Miiverse.
*/

extern "C" {
int rpl_entry(void * handle, int reason)
{
    std::printf("PLUGIN: rpl_entry. handle:%p, reason:%d\n", handle, reason);
    return 0;
}

UnityApi unity;

void UnityManageMemory(DotNetCoAllocFunc* coAlloc, DotNetCoFreeFunc* coFree)
{
    unity.CoAlloc = coAlloc;
    unity.CoFree = coFree;
    std::printf("PLUGIN: UnityManageMemory (%p, %p)\n", unity.CoAlloc, unity.CoFree);
}

// -----------------------------------------------------------

static MEMHeapHandle expHeap = MEM_HEAP_INVALID_HANDLE;

#define GET_MAX(a, b) ( ((a)>(b) ) ? (a) : (b) )

static const s32 OLV_WORK_SIZE = (64 * 1024) +  // for initialization
    GET_MAX((256 * 1024),                       // for uploading
        (256 * 1024));                          // for downloading

static void OLV_Finalize();


static u8* s_WorkMem = NULL;
static u8* p_WorkMem = NULL;

std::wstring towchar(char const* s)
{
    // TODO: use utf8_len and get rid of temp allocation
    size_t msgSize = strlen(s);
    wchar_t* body = new wchar_t[msgSize + 1];
    mbstowcs(body, s, msgSize + 1);

    std::wstring result(body);
    delete body;

    return result;
}

// TODO: fix success status
// Here we are reusing the same value to indicate success or user-error code, which in violation of guidelines
int OLV_Init()
{
    nn::Result res = nn::act::Initialize();
    if (res.IsFailure())
    {
        OSReport("failed to init account system library, %d\n", res.GetPrintableBits());
        return nn::act::GetErrorCode(res);
    }

    char actId[ACT_ACCOUNT_ID_SIZE];
    res = nn::act::GetAccountId(actId);
    if (res.IsFailure())
    {
        snprintf(actId, sizeof(actId), "<Error:%d>", res.GetPrintableBits());
        return nn::act::GetErrorCode(res);
    }

    OSReport("Working with account %d, isNetworkAcc:%d, id:%s\n", nn::act::GetSlotNo(), nn::act::IsNetworkAccount(), actId);

    s_WorkMem = (u8*)malloc(OLV_WORK_SIZE);
    if (s_WorkMem == NULL)
    {
        OSReport("PLUGIN: ERROR: Could not allocate OLV work buffer.\n");
        return CMN_MSG_FATAL;
    }

    p_WorkMem = (u8*)malloc(nn::ngc::Cafe::ProfanityFilter::WORKMEMORY_SIZE);
    if (p_WorkMem == NULL)
    {
        OSReport("PLUGIN: ERROR: Could not allocate Profanity Filter work buffer.\n");
        return CMN_MSG_FATAL;
    }

    NSSL_RVAL sslRet = NSSLInit();
    if (sslRet != NSSL_RVAL_OK)
    {
        OSReport("PLUGIN: ERROR: NSSLinit failed with NSSL_RVAL %d\n", sslRet);
        return CMN_MSG_FATAL;
    }

    nn::olv::InitializeParam initParam;
    res = initParam.SetWork(s_WorkMem, OLV_WORK_SIZE);
    if (res.IsFailure())
    {
        OSReport("PLUGIN: ERROR: setting working memory failed during OLV init! (code:%x)\n", res.GetPrintableBits());
        return nn::olv::GetErrorCode(res);
    }

    res = nn::olv::Initialize(&initParam);
    if (res.IsFailure())
    {
        OSReport("PLUGIN: ERROR: olv initialization failed! (code:%x)\n", res.GetPrintableBits());
        return nn::olv::GetErrorCode(res);
    }

    return 0;
}

void OLV_Finalize()
{
    nn::olv::Finalize();
    NSSLFinish();

    free(s_WorkMem);
    free(p_WorkMem);
}

void OLV_PostText(char* szMessage)
{
    OSReport("PLUGIN: -----------------------------------\n");
    OSReport("PLUGIN: -~= posting text: %s =~-\n", szMessage);
    OSReport("PLUGIN: -----------------------------------\n");

    nn::olv::UploadPostDataParam uploadParam;
    nn::olv::UploadedPostData uploadedData;

    std::wstring body = towchar(szMessage);
    uploadParam.SetBodyText(body.c_str());

    nn::Result result = nn::olv::UploadPostData(&uploadedData, &uploadParam);

    if (result.IsFailure())
        OSReport("PLUGIN: ERROR: Upload failed!\n");
    else
        OSReport("PLUGIN: Upload successfull!\n");

    // If the post is successful, the result after posting gets stored in uploadedData.
    // The code shown below gets the post result post ID.
    OSReport("PostId: %s\n", uploadedData.GetPostId());
}

char* OLV_FilterWord(char* szMessage)
{
    OSReport("PLUGIN: szMessage: %s\n", szMessage);
    nn::ngc::Cafe::ProfanityFilterPatternList pat = nn::ngc::Cafe::PATTERNLIST_AMERICA_ENGLISH;

    std::wstring body = towchar(szMessage);

    nn::ngc::Cafe::ProfanityFilter filter;
    nn::Result result = filter.Initialize((uptr)p_WorkMem);
    filter.MaskProfanityWordsInText(NULL, pat, const_cast<wchar_t*>(body.c_str()));

    // According to marshal rules, this buffer will be release by mono after the content
    // has been translated into the managed type.
    char* ret = (char*)unity.CoAlloc(body.size() + 1);
    wcstombs(ret, body.c_str(), body.size() + 1);

    OSReport("PLUGIN: buffer: %s\n", ret);
    return ret;
}

char* OLV_GetLatestPostedText()
{
    OSReport("PLUGIN: --------------------------------------\n");
    OSReport("PLUGIN: -~= downloading latest posted text =~-\n");
    OSReport("PLUGIN: --------------------------------------\n");

    char* ret;

    nn::olv::DownloadPostDataListParam downloadParam;
    downloadParam.SetPostDataMaxNum(1);

    downloadParam.SetFlags(0);

    nn::olv::DownloadedTopicData topicData;
    nn::olv::DownloadedPostData postDataList[1];
    u32 downloadListSize = 0;

    nn::Result result = nn::olv::DownloadPostDataList(
            &topicData, postDataList,
            &downloadListSize, 1, &downloadParam);

    if (result.IsFailure())
    {
        OSReport("PLUGIN: ERROR: Download of latest post failed!\n");
        goto failure;
    }

    OSReport("PLUGIN: Download of latest post succeeded!\n");

    if (downloadListSize == 0)
    {
        OSReport("PLUGIN: WARNING: But no post was returned!!\n");
        goto failure;
    }

    const u32 bodyTextBuffLength = nn::olv::BODY_TEXT_MAX_LENGTH + 1;
    wchar_t bodyText[bodyTextBuffLength];

    OSCalendarTime ct;
    OSTicksToCalendarTime(postDataList[0].GetPostDate(), &ct);
    OSReport("PLUGIN: Post was submitted on: %d/%d/%d %02d:%02d:%02d\n",
        ct.year, ct.mon + 1, ct.mday, ct.hour, ct.min, ct.sec);

    result = postDataList[0].GetBodyText(bodyText, bodyTextBuffLength);
    if (!result.IsSuccess())
    {
        OSReport("PLUGIN: ERROR: Can not get text from post!");
        goto failure;
    }

    size_t msgLength = wcslen(bodyText);
    // The buffer will be released by mono.
    ret = (char*)unity.CoAlloc(msgLength + 1);

    wcstombs(ret, bodyText, msgLength + 1);

    OSReport("PLUGIN: BodyText: %s\n", ret);

    return ret;

failure:
    ret = (char*)unity.CoAlloc(1);
    *ret = 0;
    return ret;
}
} // extern "C"
