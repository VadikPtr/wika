let copyEmailElPopover;
let editor;
let mdConverter;

function onPageRefreshed() {
  console.log('onPageRefreshed');

  renderMarkdown();

  const copyEmailEl = document.getElementById('copy-email');
  if (copyEmailEl) {
    copyEmailElPopover = new bootstrap.Popover(
      copyEmailEl, {trigger: 'manual', placement: 'bottom'}
    );
  }

  const editorEl = document.getElementById('editor');
  const editContentEl = document.getElementById('edit_content');
  if (!editorEl && !editContentEl && editor) {
    console.log("destroy ace editor");
    window.location.replace(location.href);
  }

  if (editorEl && editContentEl && !editContentEl.classList.contains('editorInitialized')) {
    editor = ace.edit(editorEl, {
      theme: "ace/theme/github_dark",
      mode: "ace/mode/markdown",
      autoScrollEditorIntoView: true,
      maxLines: 5000,
      tabSize: 4,
    });
    editor.setShowPrintMargin(false);
    editor.session.setUseWrapMode(true);
    editor.session.setUseSoftTabs(true);
    editor.session.on('change', function () {
      editContentEl.value = editor.getSession().getValue();
    });
    editorEl.classList.add('editorInitialized');
    editContentEl.value = editor.getSession().getValue();
  }
}

addEventListener("DOMContentLoaded", () => {
  const observer = new MutationObserver((mutationList) => {
    // skip ace editor mutation events
    if (mutationList.some(x =>
      x.target.classList &&
      [...x.target.classList.values()].some(cls => cls.includes("ace"))
    )) {
      return;
    }

    onPageRefreshed();
  });
  observer.observe(document, {subtree: true, childList: true});
  onPageRefreshed();
});

function startLogin() {
  document.getElementById("loader").classList.remove('d-none')
  document.location = "/auth/login-start";
}

function renderMarkdown() {
  if (!mdConverter) {
    mdConverter = new showdown.Converter({
      extensions: ["github", "youtube", showdownHighlight],
      emoji: true,
      simpleLineBreaks: true,
      tables: true,
      parseImgDimensions: true,
      headerLevelStart: 1,
      disableForced4SpacesIndentedSublists: true,
      tasklists: true,
    });
    mdConverter.setFlavor("github");
  }

  const mdText = document.getElementById("md-text");
  if (!mdText || mdText.classList.contains('markdownInitialized')) {
    return;
  }

  mdText.classList.add('markdownInitialized');
  document.getElementById("md-render").innerHTML = mdConverter.makeHtml(mdText.textContent);

  // TODO:
  // const mdHeader = $("#md-render > :header:first-child").text();
  // if (mdHeader.length) {
  //   document.title = mdHeader;
  // }
  // $("table").each(function () {
  //   const el = $(this);
  //   if (!el.hasClass("table")) {
  //     el.addClass("table");
  //   }
  // });
  //
  // $("blockquote").each(function () {
  //   const el = $(this);
  //   if (!el.hasClass("blockquote")) {
  //     el.addClass("blockquote");
  //   }
  // });

  document.querySelectorAll("blockquote").forEach((el) => {
    if (!el.className.includes("blockquote")) {
      el.classList.add("blockquote");
    }
  });
}
