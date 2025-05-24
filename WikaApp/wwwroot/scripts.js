let copyEmailElPopover;
let editor;

function onPageRefreshed() {
  console.log('onPageRefreshed');

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
      theme: "ace/theme/textmate",
      mode: "ace/mode/markdown",
      autoScrollEditorIntoView: true,
      maxLines: 100,
      tabSize: 2,
      // TODO: useSoftTabs: true,
      // TODO: useWrapMode: true,
    });
    editor.session.on('change', function () {
      editContentEl.value = editor.getSession().getValue();
    });
    editorEl.classList.add('editorInitialized');
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
