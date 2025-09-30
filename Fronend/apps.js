const API_BASE = "http://localhost:5187/api/patients";

const form = document.getElementById('patient-form');
const formMsg = document.getElementById('form-msg');
const search = document.getElementById('search');
const reloadBtn = document.getElementById('reload');
const tableBody = document.querySelector('#patients-table tbody');
const listMsg = document.getElementById('list-msg');

function formToPayload(formEl) {
  const data = new FormData(formEl);
  const o = Object.fromEntries(data.entries());
  // API expects PascalCase property names from our model—map here:
  return {
    firstName: o.firstName?.trim(),
    lastName: o.lastName?.trim(),
    dob: o.dob,                  // yyyy-MM-dd
    gender: o.gender,
    phone: o.phone?.trim(),
    email: o.email || null,
    addressLine1: o.addressLine1 || null,
    addressLine2: o.addressLine2 || null,
    city: o.city || null,
    state: o.state || null,
    postalCode: o.postalCode || null,
    insuranceProvider: o.insuranceProvider || null,
    insuranceNumber: o.insuranceNumber || null,
    emergencyContactName: o.emergencyContactName || null,
    emergencyContactPhone: o.emergencyContactPhone || null
  };
}

function rowHtml(p) {
  const fullName = `${p.firstName} ${p.lastName}`;
  return `
    <tr>
      <td>${p.id}</td>
      <td>${fullName}</td>
      <td>${p.dob}</td>
      <td>${p.gender}</td>
      <td>${p.phone}</td>
      <td>${p.email ?? ''}</td>
      <td>${new Date(p.createdAt || '').toLocaleString() || ''}</td>
      <td class="actions-cell">
        <button data-id="${p.id}" class="view">View</button>
        <button data-id="${p.id}" class="danger del">Delete</button>
      </td>
    </tr>
  `;
}

async function listPatients(q = '') {
  const res = await fetch(`${API_BASE}?q=${encodeURIComponent(q)}`);
  if (!res.ok) throw new Error('List failed');
  return res.json();
}

async function createPatient(payload) {
  const res = await fetch(API_BASE, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload)
  });
  return res.json();
}

async function deletePatient(id) {
  await fetch(`${API_BASE}/${id}`, { method: 'DELETE' });
}

async function refresh(q = '') {
  listMsg.textContent = 'Loading...';
  try {
    const list = await listPatients(q);
    tableBody.innerHTML = list.map(rowHtml).join('');
    listMsg.textContent = list.length ? '' : 'No records found.';
  } catch (e) {
    listMsg.textContent = 'Failed to load patients.';
  }
}

form.addEventListener('submit', async (e) => {
  e.preventDefault();
  formMsg.textContent = 'Saving...';
  try {
    const payload = formToPayload(form);
    const res = await createPatient(payload);
    if (res.error) formMsg.textContent = `Error: ${res.error}`;
    else {
      formMsg.textContent = 'Saved successfully!';
      form.reset();
      await refresh(search.value);
    }
  } catch (err) {
    formMsg.textContent = 'Save failed.';
  }
});

search.addEventListener('input', () => refresh(search.value));
reloadBtn.addEventListener('click', () => refresh(search.value));

document.addEventListener('click', async (e) => {
  if (e.target.classList.contains('del')) {
    const id = e.target.getAttribute('data-id');
    if (confirm(`Delete patient #${id}?`)) {
      await deletePatient(id);
      await refresh(search.value);
    }
  }
  if (e.target.classList.contains('view')) {
    const id = e.target.getAttribute('data-id');
    const res = await fetch(`${API_BASE}/${id}`);
    const p = await res.json();
    alert(`Patient #${p.id}\n${p.firstName} ${p.lastName}\nDOB: ${p.dob}\nPhone: ${p.phone}\nEmail: ${p.email || ''}`);
  }
});

// First load
refresh();
