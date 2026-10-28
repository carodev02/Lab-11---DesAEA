const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
const dialog = document.querySelector('#editor'), form = document.querySelector('#formProducto');
const categoriesDialog = document.querySelector('#categoriasModal'), categoryForm = document.querySelector('#formCategoria');
let items = [], categorias = [];
const money = n => new Intl.NumberFormat('es-PE',{style:'currency',currency:'PEN'}).format(n);
const dateText = date => new Intl.DateTimeFormat('es-PE',{day:'2-digit',month:'short'}).format(new Date(date));
const esc = s => { const d=document.createElement('div'); d.textContent=s||''; return d.innerHTML; };
async function request(url, options={}) { const r=await fetch(url,options); if(!r.ok){let detail={};try{detail=await r.json()}catch{} throw new Error(detail.mensaje||'No se pudo completar la operación.');} return r.json(); }
async function cargar(){
 try {
  const term=document.querySelector('#buscar').value;
  const category=document.querySelector('#filtrarCategoria').value, estado=document.querySelector('#filtrarEstado').value;
  const query=new URLSearchParams({buscar:term}); if(category)query.set('categoriaId',category);if(estado)query.set('estado',estado);
  const [p,c,s,paneles]=await Promise.all([request('/Productos/Listar?'+query),request('/Productos/Categorias'),request('/Productos/Resumen'),request('/Productos/Paneles')]);
  items=p; categorias=c;
  document.querySelector('#filas').innerHTML=items.map(x=>`<tr><td><div class="product-cell"><span class="product-icon">${esc(x.nombre.trim().charAt(0).toUpperCase())}</span><div><strong>${esc(x.nombre)}</strong><small>${esc(x.descripcion||'Sin descripción')}</small></div></div></td><td><span class="tag">${esc(x.categoria)}</span></td><td class="price">${money(x.precio)}</td><td><span class="stock ${x.stock<6?'low':''}">${x.stock} un.</span></td><td><div class="actions"><button data-edit="${x.id}">Editar</button><button class="remove" data-disable="${x.id}">Desactivar</button></div></td></tr>`).join('');
  document.querySelector('#total').textContent=s.productos; document.querySelector('#unidades').textContent=s.unidades; document.querySelector('#stockBajo').textContent=s.stockBajo;
  document.querySelector('#vacio').hidden=items.length>0;
  document.querySelector('#categoria').innerHTML='<option value="">Seleccionar categoría</option>'+categorias.map(x=>`<option value="${x.id}">${esc(x.nombre)}</option>`).join('');
  const categoryFilter=document.querySelector('#filtrarCategoria');const selected=categoryFilter.value;categoryFilter.innerHTML='<option value="">Todas las categorías</option>'+categorias.map(x=>`<option value="${x.id}">${esc(x.nombre)}</option>`).join('');categoryFilter.value=selected;
  pintarPaneles(paneles,s.productos);
  await cargarCategorias();
 } catch(e){aviso(e.message,'error');}
}
function aviso(m,tipo='ok'){const t=document.querySelector('#toast');t.textContent=m;t.className='toast show '+tipo;setTimeout(()=>t.classList.remove('show'),3000)}
function pintarPaneles(data,total){
 const colors=['#8260a0','#ac8bc2','#c5afd4','#71558b','#d8c9e1'];
 document.querySelector('#categoryChart').innerHTML=data.categorias.length?data.categorias.map((c,i)=>`<div class="chart-row"><div class="chart-label"><span>${esc(c.nombre)}</span><b>${c.total}</b></div><div class="chart-track"><i style="width:${total?Math.max(c.total/total*100,3):0}%;background:${colors[i%colors.length]}"></i></div></div>`).join(''):'<p class="subtle-empty">Aún no hay categorías para mostrar.</p>';
 document.querySelector('#alertCount').textContent=data.alertas.length;
 document.querySelector('#lowStockList').innerHTML=data.alertas.length?data.alertas.map(p=>`<div class="low-stock-row"><span class="warning-dot">!</span><div class="low-copy"><strong>${esc(p.nombre)}</strong><small>${esc(p.categoria)}</small></div><span class="units-left">${p.stock} un.</span><button class="restock" data-restock="${p.id}" title="Agregar cinco unidades">+5</button></div>`).join(''):'<p class="subtle-empty success-empty">Todo el inventario está abastecido.</p>';
 document.querySelector('#recentList').innerHTML=data.recientes.length?data.recientes.map(p=>`<div class="recent-row"><span class="recent-mark">✦</span><div class="recent-copy"><strong>${esc(p.nombre)}</strong><small>${esc(p.categoria)}</small></div><time>${dateText(p.fechaRegistro)}</time></div>`).join(''):'<p class="subtle-empty">Aún no hay actividad registrada.</p>';
}
function abrir(){form.reset();form.elements.Id.value=0;document.querySelector('#tituloModal').textContent='Nuevo producto';cargar().then(()=>dialog.showModal())}
async function editar(id){const x=items.find(a=>a.id===id);if(!x)return;form.reset();form.elements.Id.value=x.id;form.elements.Nombre.value=x.nombre;form.elements.Descripcion.value=x.descripcion||'';form.elements.Precio.value=x.precio;form.elements.Stock.value=x.stock;form.elements.CategoriaId.value=x.categoriaId;document.querySelector('#tituloModal').textContent='Editar producto';dialog.showModal()}
async function desactivar(id){if(!confirm('¿Desactivar este producto? Se conservará su registro.'))return;const d=new FormData();d.append('id',id);d.append('__RequestVerificationToken',token);try{await request('/Productos/Desactivar',{method:'POST',body:d});aviso('Producto desactivado');cargar()}catch(e){aviso(e.message,'error')}}
async function cargarCategorias(){const rows=await request('/Categorias/Listar');document.querySelector('#listaCategorias').innerHTML=rows.map(c=>`<div class="category-row"><span class="category-symbol">▦</span><div class="category-copy"><strong>${esc(c.nombre)}</strong><small>${esc(c.descripcion||'')} · ${c.productos} productos</small></div><button class="remove-category" data-category-disable="${c.id}" ${c.productos?'disabled title="Desactiva o cambia los productos de esta categoría primero"':''}>Desactivar</button></div>`).join('')||'<p class="category-empty">Aún no hay categorías.</p>';}
document.querySelector('#nuevo').onclick=abrir;document.querySelector('#cerrar').onclick=()=>dialog.close();document.querySelector('#cancelar').onclick=()=>dialog.close();
document.querySelector('#buscar').oninput=()=>{clearTimeout(window.searchTimer);window.searchTimer=setTimeout(cargar,220)};
document.querySelector('#filtrarCategoria').onchange=cargar;document.querySelector('#filtrarEstado').onchange=cargar;
document.querySelector('#filas').addEventListener('click',e=>{const edit=e.target.closest('[data-edit]'),disable=e.target.closest('[data-disable]');if(edit)editar(Number(edit.dataset.edit));if(disable)desactivar(Number(disable.dataset.disable));});
document.querySelector('#lowStockList').addEventListener('click',async e=>{const b=e.target.closest('[data-restock]');if(!b)return;const d=new FormData();d.append('id',b.dataset.restock);d.append('cantidad',5);d.append('__RequestVerificationToken',token);try{await request('/Productos/Reponer',{method:'POST',body:d});aviso('Se agregaron 5 unidades al inventario');await cargar()}catch(e){aviso(e.message,'error')}});
form.onsubmit=async e=>{e.preventDefault();const d=new FormData(form);d.append('__RequestVerificationToken',token);try{await request('/Productos/Guardar',{method:'POST',body:d});dialog.close();aviso('Producto guardado');cargar()}catch(e){aviso(e.message,'error')}};
document.querySelector('#gestionarCategorias').onclick=async()=>{await cargarCategorias();categoriesDialog.showModal()};document.querySelector('#cerrarCategorias').onclick=()=>categoriesDialog.close();
categoryForm.onsubmit=async e=>{e.preventDefault();const d=new FormData(categoryForm);d.append('__RequestVerificationToken',token);try{await request('/Categorias/Guardar',{method:'POST',body:d});categoryForm.reset();categoryForm.elements.Id.value=0;aviso('Categoría agregada');await cargar()}catch(e){aviso(e.message,'error')}};
document.querySelector('#listaCategorias').addEventListener('click',async e=>{const b=e.target.closest('[data-category-disable]');if(!b||b.disabled)return;if(!confirm('¿Desactivar esta categoría?'))return;const d=new FormData();d.append('id',b.dataset.categoryDisable);d.append('__RequestVerificationToken',token);try{await request('/Categorias/Desactivar',{method:'POST',body:d});aviso('Categoría desactivada');await cargar()}catch(e){aviso(e.message,'error')}});
cargar();
