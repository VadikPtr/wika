let copyEmailElPopover;

function startLogin() {
  document.getElementById("loader").classList.remove('d-none')
  document.location = "/auth/login-start";
}

addEventListener("DOMContentLoaded", () => {
  const copyEmailEl = document.getElementById('copy-email');
  if (copyEmailEl) {
    copyEmailElPopover = new bootstrap.Popover(
      copyEmailEl, {trigger: 'manual', placement: 'bottom'}
    );
  }
});
